namespace ShootingAcademy.Services
{
    public static class AutorizeData
    {
        public class AData
        {
            private readonly HttpContext context;

            public Guid UserGuid => Guid.Parse(context.User.Claims.ToArray()[0].Value);
            public bool IsAutorize => context.User.Claims.Any();
            public string Role => context.User.Claims.ToArray()[1].Value;

            public AData(HttpContext context)
            {
                this.context = context;
            }
        }

        public static AData FromContext(HttpContext context)
        {
            return new AData(context);
        }
    }
}
