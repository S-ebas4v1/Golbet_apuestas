namespace GolBet.Services.Exceptions
{
    // Violación de una regla de negocio. Es un tipo propio para que los controladores
    // la atrapen sin tragarse por accidente otras InvalidOperationException (EF lanza varias).
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
