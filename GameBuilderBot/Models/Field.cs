using System;

namespace GameBuilderBot.Models
{
    public class Field
    {
        public readonly string Expression;
        public object Value;
        public Type Type;

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
            Expression = expression;
            Value = value;
            switch(type)
            {
                case "int":
                case "integer":
                    Type = typeof(int);
                    Value = Convert.ToInt64(value);
                    break;
                default:
                    string msg = "Type " + type + " not implemented in Fields.";
                    throw new System.InvalidOperationException(msg);
            }
        }
    }
}
