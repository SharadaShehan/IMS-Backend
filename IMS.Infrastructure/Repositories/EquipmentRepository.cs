using System.Diagnostics;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly DataBaseContext _dbContext;

        public EquipmentRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Equipment? GetEquipmentEntityById(int id)
        {
            return _dbContext
                .equipments.Where(e => e.EquipmentId == id && e.IsActive)
                .FirstOrDefault();
        }

        public EquipmentDetailedDTO? GetEquipmentDTOById(int id)
        {
            int totalCount = _dbContext.items.Where(i => i.EquipmentId == id && i.IsActive).Count();
            int reservedCount = _dbContext
                .itemReservations.Where(ir =>
                    ir.EquipmentId == id
                    && (ir.Status == "Reserved" || ir.Status == "Borrowed")
                    && ir.IsActive
                )
                .Count();
            // availableCount = number of items currently physically available in the lab
            int availableCount = _dbContext
                .items.Where(i => i.EquipmentId == id && i.IsActive && i.Status == "Available")
                .Count();
            return _dbContext
                .equipments.Where(e => e.EquipmentId == id && e.IsActive)
                .Select(e => new EquipmentDetailedDTO
                {
                    equipmentId = e.EquipmentId,
                    name = e.Name,
                    model = e.Model,
                    imageUrl = e.ImageURL,
                    labId = e.LabId,
                    labName = e.Lab.LabName,
                    specification = e.Specification,
                    maintenanceIntervalDays = e.MaintenanceIntervalDays,
                    totalCount = totalCount,
                    reservedCount = reservedCount,
                    availableCount = availableCount,
                })
                .FirstOrDefault();
        }

        public List<EquipmentDTO> GetAllEquipmentDTOs(int labId)
        {
            return _dbContext
                .equipments.Where(e => e.IsActive && e.LabId == labId)
                .Select(e => new EquipmentDTO
                {
                    equipmentId = e.EquipmentId,
                    name = e.Name,
                    model = e.Model,
                    imageUrl = e.ImageURL,
                    labId = e.LabId,
                    labName = e.Lab.LabName,
                    specification = e.Specification,
                    maintenanceIntervalDays = e.MaintenanceIntervalDays,
                })
                .ToList();
        }

        public bool CheckIfEquipmentExists(string name, string model, int labId)
        {
            Equipment? existingEquipment = _dbContext
                .equipments.Where(e =>
                    e.Name == name && e.Model == model && e.LabId == labId && e.IsActive
                )
                .FirstOrDefault();
            return existingEquipment != null;
        }

        public EquipmentDetailedDTO? CreateNewEquipment(
            CreateEquipmentDTO createEquipmentDTO,
            Lab lab
        )
        {
            Equipment equipment = new Equipment
            {
                Name = createEquipmentDTO.name,
                Model = createEquipmentDTO.model,
                LabId = lab.LabId,
                Lab = lab,
                ImageURL = createEquipmentDTO.imageURL,
                Specification = createEquipmentDTO.specification,
                MaintenanceIntervalDays = createEquipmentDTO.maintenanceIntervalDays,
                IsActive = true,
            };
            _dbContext.equipments.Add(equipment);
            _dbContext.SaveChanges();
            return GetEquipmentDTOById(equipment.EquipmentId);
        }

        public EquipmentDetailedDTO? UpdateEquipment(
            Equipment equipment,
            UpdateEquipmentDTO updateEquipmentDTO
        )
        {
            if (equipment == null)
                return null;
            if (updateEquipmentDTO.name != null)
                equipment.Name = updateEquipmentDTO.name;
            if (updateEquipmentDTO.model != null)
                equipment.Model = updateEquipmentDTO.model;
            if (updateEquipmentDTO.imageURL != null)
                equipment.ImageURL = updateEquipmentDTO.imageURL;
            if (updateEquipmentDTO.specification != null)
                equipment.Specification = updateEquipmentDTO.specification;
            if (updateEquipmentDTO.maintenanceIntervalDays != null)
                equipment.MaintenanceIntervalDays = (int)updateEquipmentDTO.maintenanceIntervalDays;
            _dbContext.Update(equipment);
            _dbContext.SaveChanges();
            return GetEquipmentDTOById(equipment.EquipmentId);
        }

        public bool DeleteEquipment(int id)
        {
            // delete equipment
            _dbContext
                .equipments.Where(e => e.EquipmentId == id)
                .ExecuteUpdate(e => e.SetProperty(e => e.IsActive, false));
            // delete items of the equipment
            _dbContext
                .items.Where(i => i.EquipmentId == id)
                .ExecuteUpdate(i => i.SetProperty(i => i.IsActive, false));
            // delete item reservations of the equipment
            _dbContext
                .itemReservations.Where(ir => ir.EquipmentId == id)
                .ExecuteUpdate(ir => ir.SetProperty(ir => ir.IsActive, false));
            // delete maintenance logs of the equipment
            _dbContext
                .maintenances.Where(m => m.Item.EquipmentId == id)
                .ExecuteUpdate(m => m.SetProperty(m => m.IsActive, false));
            _dbContext.SaveChanges();
            return true;
        }
    }
}
