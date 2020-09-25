namespace GameBuilderBot.Models
{
    public class Field
    {
        public readonly string Expression;
        public int? Value;

        public Field(FieldIngest f)
        {
            Expression = f.Expression;
            Value = f.Value;
        }

        public Field(string expression, int? value)
        {
            Expression = expression;
            Value = value;
        }
    }
}
