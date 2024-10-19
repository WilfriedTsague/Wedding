using System.ComponentModel.DataAnnotations.Schema;

namespace Wedding.Models
{
    public class Invite
    {

        public int Id { get; set; }
        public string NomInvite { get; set; }
        public string PrenomInvite { get; set; }
        public int? IdInviteur { get; set; }
        public Invite? Inviteur { get; set; }
        public int IdTable { get; set; }
        public virtual Table? Table { get; set; }


        public string TypeBillets { get; set; }
        public string? QRCodeId { get; set; }

    }

    //public enum TypeBillets { Solo, Couple }
}
