namespace GameBuilderBot.Models
{
    public class FieldIngest
    {
        public string Expression;
        public int? Value;

        public FieldIngest() { }

        public FieldIngest(string expression, int? value)
        {
            Expression = expression;
            Value = value;
        }

    }
}
