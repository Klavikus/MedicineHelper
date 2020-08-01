
using System.ComponentModel.DataAnnotations.Schema;
namespace Medicine.db
{
    public class User
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string NickName { get; set; }
        public int LastCommandIndex { get; set; }
        public string LastCommand { get; set; }
    }
}
