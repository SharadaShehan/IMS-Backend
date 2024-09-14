using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Repositories
{
    public class LabRepository : ILabRepository
    {
        private readonly DataBaseContext _dbContext;

        public LabRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Lab? GetLabEntityById(int id)
        {
            return _dbContext.labs.Where(l => l.LabId == id && l.IsActive).FirstOrDefault();
        }

        public LabDTO? GetLabDTOById(int id)
        {
            return _dbContext
                .labs.Where(l => l.LabId == id && l.IsActive)
                .Select(l => new LabDTO
                {
                    labId = l.LabId,
                    labName = l.LabName,
                    labCode = l.LabCode,
                    imageUrl = l.ImageURL,
                })
                .FirstOrDefault();
        }

        public List<LabDTO> GetAllLabDTOs()
        {
            return _dbContext
                .labs.Where(l => l.IsActive)
                .Select(l => new LabDTO
                {
                    labId = l.LabId,
                    labName = l.LabName,
                    labCode = l.LabCode,
                    imageUrl = l.ImageURL,
                })
                .ToList();
        }

        public bool CheckIfLabExists(string labName, string labCode)
        {
            Lab? existingLab = _dbContext
                .labs.Where(l => l.LabName == labName && l.LabCode == labCode && l.IsActive)
                .FirstOrDefault();
            return existingLab != null;
        }

        public LabDTO? CreateNewLab(CreateLabDTO createLabDTO)
        {
            Lab lab = new Lab
            {
                LabName = createLabDTO.labName,
                LabCode = createLabDTO.labCode,
                ImageURL = createLabDTO.imageURL,
                IsActive = true,
            };
            _dbContext.labs.Add(lab);
            _dbContext.SaveChanges();
            return GetLabDTOById(lab.LabId);
        }

        public LabDTO? UpdateLab(Lab lab, UpdateLabDTO updateLabDTO)
        {
            if (lab == null)
                return null;
            if (updateLabDTO.labName != null)
                lab.LabName = updateLabDTO.labName;
            if (updateLabDTO.labCode != null)
                lab.LabCode = updateLabDTO.labCode;
            if (updateLabDTO.imageURL != null)
                lab.ImageURL = updateLabDTO.imageURL;
            _dbContext.Update(lab);
            _dbContext.SaveChanges();
            return GetLabDTOById(lab.LabId);
        }

        public bool DeleteLab(int id)
        {
            // delete lab
            _dbContext
                .labs.Where(l => l.LabId == id)
                .ExecuteUpdate(l => l.SetProperty(l => l.IsActive, false));
            // delete equipments of the lab
            _dbContext
                .equipments.Where(e => e.LabId == id)
                .ExecuteUpdate(e => e.SetProperty(e => e.IsActive, false));
            // delete items of the lab
            _dbContext
                .items.Where(i => i.Equipment.LabId == id)
                .ExecuteUpdate(i => i.SetProperty(i => i.IsActive, false));
            // delete item reservations of the lab
            _dbContext
                .itemReservations.Where(r => r.Equipment.LabId == id)
                .ExecuteUpdate(r => r.SetProperty(r => r.IsActive, false));
            // delete maintenances of the lab
            _dbContext
                .maintenances.Where(m => m.Item.Equipment.LabId == id)
                .ExecuteUpdate(m => m.SetProperty(m => m.IsActive, false));
            _dbContext.SaveChanges();
            return true;
        }
    }
}
