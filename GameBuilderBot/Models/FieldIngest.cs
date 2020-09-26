namespace GameBuilderBot.Models
{
    public class FieldIngest
    {
        public string Expression;
        public object Value;
        public string Type;

        public FieldIngest() { }

        public FieldIngest(string expression, object value, string type = "int")
        {
            Expression = expression;
            Value = value;
            Type = type;
        }

    }
}
