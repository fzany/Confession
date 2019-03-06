namespace Uwp.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Guid { get; set; } = Store.Helpers.ObjectIds.GenerateNewId().ToString();
        public string Name { get; set; } = string.Empty;
    }
}
