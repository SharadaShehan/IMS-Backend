using IMS.Application.DTO;
using IMS.Core.Model;

namespace IMS.Application.Interfaces
{
    public interface ILabRepository
    {
        Lab? GetLabEntityById(int id);
        LabDTO? GetLabDTOById(int id);
        List<LabDTO> GetAllLabDTOs();
        bool CheckIfLabExists(string labName, string labCode);
        LabDTO? CreateNewLab(CreateLabDTO createLabDTO);
        LabDTO? UpdateLab(Lab lab, UpdateLabDTO updateLabDTO);
        bool DeleteLab(int id);
    }
}
