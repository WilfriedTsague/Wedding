using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System.Drawing.Text;
using Wedding.ContexteDB;
using Wedding.Models;
using QRCoder;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using System.Drawing; // Assurez-vous d'avoir ajouté System.Drawing.Common via NuGet




namespace Wedding.Controllers
{
    public class InvitesController : Controller
    {
        private readonly WeddingContext _context;

        public InvitesController(WeddingContext context)
        {
            _context = context;
        }
        // GET: Invites
        public async Task<IActionResult> Index(string sortOrder, string searchNomInvite, string searchPrenomInvite, string searchNomTable)
        {
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NomInviteSortParm = sortOrder == "NomInvite" ? "nominvite_desc" : "NomInvite";
            ViewBag.PrenomInviteSortParm = sortOrder == "PrenomInvite" ? "prenominvite_desc" : "PrenomInvite";
            ViewBag.InviteurPrenomSortParm = sortOrder == "InviteurPrenom" ? "inviteurprenom_desc" : "InviteurPrenom";
            ViewBag.NomTableSortParm = sortOrder == "NomTable" ? "nomtable_desc" : "NomTable";
            ViewBag.TypeBilletsSortParm = sortOrder == "TypeBillets" ? "typebillets_desc" : "TypeBillets";

            var invites = from i in _context.Invites.Include(i => i.Inviteur).Include(i => i.Table)
                          select i;

            if (!string.IsNullOrEmpty(searchNomInvite))
            {
                invites = invites.Where(i => i.NomInvite.Contains(searchNomInvite));
            }

            if (!string.IsNullOrEmpty(searchPrenomInvite))
            {
                invites = invites.Where(i => i.PrenomInvite.Contains(searchPrenomInvite));
            }

            if (!string.IsNullOrEmpty(searchNomTable))
            {
                invites = invites.Where(i => i.Table.NomTable.Contains(searchNomTable));
            }

            switch (sortOrder)
            {
                case "id_desc":
                    invites = invites.OrderByDescending(i => i.Id);
                    break;
                case "NomInvite":
                    invites = invites.OrderBy(i => i.NomInvite);
                    break;
                case "nominvite_desc":
                    invites = invites.OrderByDescending(i => i.NomInvite);
                    break;
                case "PrenomInvite":
                    invites = invites.OrderBy(i => i.PrenomInvite);
                    break;
                case "prenominvite_desc":
                    invites = invites.OrderByDescending(i => i.PrenomInvite);
                    break;
                case "InviteurPrenom":
                    invites = invites.OrderBy(i => i.Inviteur.PrenomInvite);
                    break;
                case "inviteurprenom_desc":
                    invites = invites.OrderByDescending(i => i.Inviteur.PrenomInvite);
                    break;
                case "NomTable":
                    invites = invites.OrderBy(i => i.Table.NomTable);
                    break;
                case "nomtable_desc":
                    invites = invites.OrderByDescending(i => i.Table.NomTable);
                    break;
                case "TypeBillets":
                    invites = invites.OrderBy(i => i.TypeBillets);
                    break;
                case "typebillets_desc":
                    invites = invites.OrderByDescending(i => i.TypeBillets);
                    break;
                default:
                    invites = invites.OrderBy(i => i.Id);
                    break;
            }

            ViewData["searchNomInvite"] = searchNomInvite;
            ViewData["searchPrenomInvite"] = searchPrenomInvite;
            ViewData["searchNomTable"] = searchNomTable;

            return View(await invites.ToListAsync());
        }


        // GET: Invites/Details/5
        public ActionResult Details(int id)
        {
            var invite = _context.Invites.Include(i => i.Table).FirstOrDefault(i => i.Id == id);

            // Vérifier si l'invité existe
            if (invite == null)
            {
                return NotFound("Invité introuvable.");
            }

            // Passer l'invité à la vue pour afficher ses détails
            return View(invite);
        }

        // GET: InvitesController/Create
        public ActionResult Create()
        {
            ViewData["IdTable"] = new SelectList(_context.Tables.Where(t => t.Statut != Statut.Pleine), "Id", "NomTable");
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "Id", "NomInvite");
            return View();
        }

        // POST: InvitesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       public async Task<IActionResult> Create([Bind("NomInvite","PrenomInvite","IdTable", "IdInviteur", "TypeBillets")] Invite invites) 
{
            if (ModelState.IsValid)     
            {   
                var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == invites.IdTable);

                if (table != null)
                {
                    int placesRequired = invites.TypeBillets == "Couple" ? 2 : 1;

                    if (table.NombreInvites + placesRequired <= table.NbrePlaces)
                    {
                        table.NombreInvites += placesRequired;

                        if (table.NombreInvites == placesRequired)
                        {
                            table.Statut = Statut.En_cours;
                        }

                        if (table.NombreInvites >= table.NbrePlaces)
                        {
                            table.Statut = Statut.Pleine;
                        }

                        _context.Update(table);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Pas assez de places disponibles sur cette table.");
                        return View(invites);
                    }
                }
                    _context.Add(invites);         
                await _context.SaveChangesAsync();         
                return RedirectToAction(nameof(Index));     
            }     
            ViewData["IdTable"] = new SelectList(_context.Tables.Where(t => t.Statut != Statut.Pleine), "Id", "NomTable"); // 
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "Id", "NomInvite");     
            return View(invites); 
        }

        // GET: InvitesController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites.FindAsync(id);
            if(invite == null)
            {  return NotFound(); }
            ViewData["IdTable"] = new SelectList(_context.Tables.Where(t => t.Statut != Statut.Pleine), "Id", "NomTable");
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "Id", "NomInvite");

            return View(invite);
        }

        // POST: InvitesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NomInvite", "PrenomInvite", "IdTable", "IdInviteur", "TypeBillets")] Invite invite)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var inviteToUpdate = await _context.Invites.Include(i => i.Table).FirstOrDefaultAsync(i => i.Id == id);
                    if (inviteToUpdate == null)
                    {
                        return NotFound();
                    }

                    var oldTable = await _context.Tables.FirstOrDefaultAsync(t => t.Id == inviteToUpdate.IdTable);
                    var newTable = await _context.Tables.FirstOrDefaultAsync(t => t.Id == invite.IdTable);

                    int oldPlacesRequired = inviteToUpdate.TypeBillets == "Couple" ? 2 : 1;
                    int newPlacesRequired = invite.TypeBillets == "Couple" ? 2 : 1;

                    // Décrémenter les places de l'ancienne table             
                    if (oldTable != null)
                    {
                        oldTable.NombreInvites -= oldPlacesRequired;
                        if (oldTable.Statut == Statut.Pleine && oldTable.NombreInvites < oldTable.NbrePlaces)
                        {
                            oldTable.Statut = Statut.En_cours;
                        }
                        _context.Update(oldTable);
                    }

                    // Vérifier et mettre à jour la nouvelle table             
                    if (newTable != null)
                    {
                        if (newTable.NombreInvites + newPlacesRequired > newTable.NbrePlaces)
                        {
                            ModelState.AddModelError("", "Impossible de changer le type de billet car la table est déjà pleine.");
                            ViewData["IdTable"] = new SelectList(_context.Tables.Where(t => t.Statut != Statut.Pleine), "Id", "NomTable", invite.IdTable);
                            ViewData["IdInviteur"] = new SelectList(_context.Invites, "Id", "NomInvite", invite.IdInviteur);
                            return View(invite);
                        }

                        newTable.NombreInvites += newPlacesRequired;
                        if (newTable.NombreInvites == newTable.NbrePlaces)
                        {
                            newTable.Statut = Statut.Pleine;
                        }
                        else if (newTable.Statut != Statut.En_cours)
                        {
                            newTable.Statut = Statut.En_cours;
                        }
                        _context.Update(newTable);
                    }

                    inviteToUpdate.NomInvite = invite.NomInvite;
                    inviteToUpdate.PrenomInvite = invite.PrenomInvite;
                    inviteToUpdate.IdTable = invite.IdTable;
                    inviteToUpdate.IdInviteur = invite.IdInviteur;
                    inviteToUpdate.TypeBillets = invite.TypeBillets;
                    _context.Update(inviteToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdTable"] = new SelectList(_context.Tables.Where(t => t.Statut != Statut.Pleine), "Id", "NomTable", invite.IdTable);
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "Id", "NomInvite", invite.IdInviteur);
            return View(invite);
        }



        // GET: InvitesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if(id == null)
            {  return NotFound(); }

            var invite = await _context.Invites.Include(i => i.Table)
                .Include(id => id.Inviteur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(invite == null)
                { return NotFound(); }
             return View(invite);
        }

        // POST: InvitesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id )
        {
          var invite = await _context.Invites.FindAsync(id);
            _context.Invites.Remove(invite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmerPresence(int inviteId)
        {
            var invite = await _context.Invites.Include(i => i.Table).FirstOrDefaultAsync(i => i.Id == inviteId);

            if (invite == null)
            { return NotFound(); }

            var table = invite.Table;

            var nombreInvitesConfirmes = _context.Invites.Count(i => i.IdTable == table.Id );
            table.StatutDuJour = nombreInvitesConfirmes == 0 ? Statut.En_cours:nombreInvitesConfirmes == table.NbrePlaces ? Statut.Pleine: table.StatutDuJour;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int inviteID)
        {
            return _context.Invites.Any(e => e.Id == inviteID);
        }


        public IActionResult GenerateQRCode(int id)
        {
            var invite = _context.Invites.Include(i => i.Table).FirstOrDefault(i => i.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            // Générer un identifiant unique pour le QR code si non défini
            if (string.IsNullOrEmpty(invite.QRCodeId))
            {
                invite.QRCodeId = Guid.NewGuid().ToString();
                _context.SaveChanges(); // Sauvegarder dans la base de données
            }

            // Informations à encoder dans le QR code
            //string qrInfo = $"ID: {invite.QRCodeId}\nIdInvite: {invite.Id}\nNom: {invite.NomInvite}\nPrenom: {invite.PrenomInvite}\nNom de Table: {invite.Table.NomTable}\nType de Billets: {invite.TypeBillets}";
            string qrInfo = $" ID:{invite.QRCodeId};IdInvite:{invite.Id};Nom:{invite.NomInvite};Prenom:{invite.PrenomInvite};NomTable:{invite.Table.NomTable};TypeBillets:{invite.TypeBillets}";

            // Génération du QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrInfo, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            // Nom du fichier avec nom et prénom de l'invité
            string fileName = $"{invite.NomInvite}_{invite.PrenomInvite}.png";

            // Chemin complet du fichier
            string folderPath = Path.Combine("wwwroot", "qrcodes");
            string filePath = Path.Combine(folderPath, fileName);

            // Vérifiez et créez le répertoire si nécessaire
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Sauvegarde du QR code avec le nom de fichier spécifique
            System.IO.File.WriteAllBytes(filePath, qrCodeImage);

            return File(qrCodeImage, "image/png", fileName);
        }


        [HttpPost]
        public IActionResult ReadQRCode(IFormFile qrCodeImage)
        {
            try
            {
                using (var ms = new MemoryStream())
                {

                    if (qrCodeImage == null)
                    {
                        return Json(new { success = false, message = "Aucune image de QR code reçue." });
                    }

                    qrCodeImage.CopyTo(ms);
                    var bitmap = new Bitmap(ms);

                    // Utilisation de ZXing pour lire le QR code
                    var reader = new BarcodeReader();
                    var result = reader.Decode(bitmap);
                    if (result == null)
                    {
                        return Json(new { success = false, message = "Impossible de lire le QR code." });
                    }


                    if (result != null)
                    {
                        // Récupère le texte décodé du QR code
                        string decodedText = result.Text;

                        // Supposons que le QR code contient un texte du genre "IdInvite:123"
                        var parts = decodedText.Split(';');
                        var idPart = parts.FirstOrDefault(p => p.StartsWith("IdInvite:"));

                        if (parts.Length == 0 || !idPart.StartsWith("IdInvite:"))
                        {
                            return Json(new { success = false, message = "Le format du QR code est incorrect." });
                        }

                        if (idPart != null)
                        {
                            int inviteId;
                            if (int.TryParse(idPart.Replace("IdInvite:", ""), out inviteId))
                            {
                                // Vérifier si l'invité existe dans la base de données
                                var invite = _context.Invites.FirstOrDefault(i => i.Id == inviteId);
                                if (invite != null)
                                {
                                    // Retourne une réponse JSON avec l'ID de l'invité
                                    return Json(new { success = true, inviteId = invite.Id });
                                }
                            }
                        }

                        return Json(new { success = false, message = "Invité non trouvé." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Impossible de lire le QR code." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguer l'exception
                Console.WriteLine(ex); // Ou utilisez un logger approprié
                return Json(new { success = false, message = $"Erreur lors du traitement du QR code : {ex.Message}" });

            }
        }





    }
}
