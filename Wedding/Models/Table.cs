using System.ComponentModel.DataAnnotations.Schema;

namespace Wedding.Models
{
    public class Table
    {

        public int Id { get; set; }
        public string NomTable { get; set; }
        public int NbrePlaces { get; set; }
        public Statut Statut { get; set; }
        public int? NbreInvitePresent { get; set; }
        public int? NombreInvites { get; set; }
        public Statut StatutDuJour { get; set; }
        public virtual ICollection<Invite>? Invites { get; set; }
    }

    //statut pour l'enregistrement des invité sur les billets

    //statut du jour de l'evement pour confirmer l'occupation des tables par les invites
    public enum Statut { Vide, En_cours, Pleine }
}
