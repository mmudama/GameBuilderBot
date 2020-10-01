using System;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// Represents a variable that is part of a game's active state. Probably this class
    /// should be renamed to "Variable" or something.
    /// <seealso cref="GameState"/><seealso cref="GameFile"/>
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Optional member suggesting the way to calculate the value. Expression could be
        /// "1d4 + 6" or "#Gallons# * #MPG#" ... or just null. This will need to be 
        /// re-evaluated when Value supports non-numeric types
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Value of a Field. Currently, non-int Values may produce unexpected results.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The Type of the Value object
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Default constructor required for deserialization
        /// </summary>
        public Field() { }

        /// <summary>
        /// Used when values are loaded from a <seealso cref="GameFile"/>
        /// <seealso cref="FieldIngest"/>
        /// </summary>
        /// <param name="f"></param>
        public Field(FieldIngest f)
        {
            Expression = f.Expression;
            Value = f.Value;
        }

        /// <summary>
        /// Constructore for the Field class.
        /// </summary>
        /// <param name="expression">Deprecated, to be removed.</param>
        /// <param name="value">String value of the field (read from JSON/YAML when loading from configuration).</param>
        /// <param name="type">The type (int, datetime, ...) of the field.  Defaults to int for backwards compatibility, change in future?</param>
        public Field(string expression, string value, string type = "int")
        {
            Expression = expression;  //Deprecated, to be moved to choices and other locations.

            switch (type.ToLower())
            {
                case "int":
                case "integer":
                    Type = typeof(int);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        Value = Convert.ToInt32(1);
                    }
                    else
                    {
                        Value = Convert.ToInt32(value);
                    }
                    break;

                case "string":
                    Type = typeof(string);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        Value = "";
                    }
                    else
                    {
                        Value = value;
                    }
                    break;

                case "datetime":
                    Type = typeof(DateTime);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        Value = new DateTime(0);
                    }
                    else if (value.ToLower().Equals("now"))
                    {
                        Value = DateTime.Now;
                    }
                    else
                    {
                        Value = Convert.ToDateTime(value);
                    }
                    break;

                default:
                    string msg = "Type " + type + " not implemented in Fields.";
                    throw new System.InvalidOperationException(msg);
            }
        }

        public void AddTo(object valueToAdd)
        {
            if (Value.GetType().Equals(typeof(string)))
            {
                throw new System.InvalidOperationException("Not Implemented.");
            }
            else if (Value.GetType().Equals(typeof(int)))
            {
                throw new System.InvalidOperationException("Not Implemented.");
            }
            else if (Value.GetType().Equals(typeof(DateTime)))
            {
                AddToDateTime(valueToAdd);
            }
            else
            {
                string msg = "Field type " + Value.GetType().ToString() + " not supported in Field.AddTo.";
                throw new System.InvalidOperationException(msg);
            }
        }

        public void AddToDateTime(object valueToAdd)
        {
            //Assume that when adding values the expression evaluator has converted anything time related into a datetime and just add ticks.
            if (!valueToAdd.GetType().Equals(typeof(DateTime)))
            {
                string msg = "Adding type " + Value.GetType().ToString() + " not supported for DateTime fields.";
                throw new System.InvalidOperationException(msg);
            }

            DateTime Old = (DateTime)Value;
            DateTime ToAdd = (DateTime)valueToAdd;

            DateTime NewValue = new DateTime(Old.Ticks + ToAdd.Ticks);

            Value = NewValue;
        }
    }
}
