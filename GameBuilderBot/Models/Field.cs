namespace GameBuilderBot.Models
{
    public class Field
    {
        public string Expression;
        public int? Value;

        public Field(FieldIngest f)
        {
            Expression = f.Expression;
            Value = f.Value;
        }
    }
}
