namespace Api
{
    public class EndpointAuthenticationDeclaration
    {
        public static void Anonymous(params IEndpointConventionBuilder[] ecb)
        {
            foreach (var e in ecb)
            {
                e.AllowAnonymous();
            }
        }

        public static void Admin(params IEndpointConventionBuilder[] ecb)
        {
            foreach (var e in ecb)
            {
                e.RequireAuthorization("admin");
            }
        }
    }
}
