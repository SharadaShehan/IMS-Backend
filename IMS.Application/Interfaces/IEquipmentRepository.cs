using IMS.Application.DTO;
using IMS.Core.Model;

namespace IMS.Application.Interfaces
{
    public interface IEquipmentRepository
    {
        Equipment? GetEquipmentEntityById(int id);
        EquipmentDetailedDTO? GetEquipmentDTOById(int id);
        List<EquipmentDTO> GetAllEquipmentDTOs(int labId);
        bool CheckIfEquipmentExists(string name, string model, int labId);
        EquipmentDetailedDTO? CreateNewEquipment(CreateEquipmentDTO createEquipmentDTO, Lab lab);
        EquipmentDetailedDTO? UpdateEquipment(
            Equipment equipment,
            UpdateEquipmentDTO updateEquipmentDTO
        );
        bool DeleteEquipment(int id);
    }
}
