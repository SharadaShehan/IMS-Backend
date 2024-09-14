using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Services
{
    public class LabService
    {
        private readonly ILabRepository _labRepository;

        public LabService(ILabRepository labRepository)
        {
            _labRepository = labRepository;
        }

        public LabDTO? GetLabById(int id)
        {
            return _labRepository.GetLabDTOById(id);
        }

        public List<LabDTO> GetAllLabs()
        {
            return _labRepository.GetAllLabDTOs();
        }

        public ResponseDTO<LabDTO> CreateNewLab(CreateLabDTO createLabDTO)
        {
            // Prevent Duplicate Lab Creation
            if (_labRepository.CheckIfLabExists(createLabDTO.labName, createLabDTO.labCode))
                return new ResponseDTO<LabDTO>("Lab Already Exists");
            // Create the Lab
            LabDTO? labDTO = _labRepository.CreateNewLab(createLabDTO);
            if (labDTO == null)
                return new ResponseDTO<LabDTO>("Failed to Create Lab");
            return new ResponseDTO<LabDTO>(labDTO);
        }

        public ResponseDTO<LabDTO> UpdateLab(int id, UpdateLabDTO updateLabDTO)
        {
            // Get the Lab to be Updated
            Lab? lab = _labRepository.GetLabEntityById(id);
            if (lab == null)
                return new ResponseDTO<LabDTO>("Lab Not Found");
            // Update the Lab
            LabDTO? updatedLabDTO = _labRepository.UpdateLab(lab, updateLabDTO);
            if (updatedLabDTO == null)
                return new ResponseDTO<LabDTO>("Failed to Update Lab");
            return new ResponseDTO<LabDTO>(updatedLabDTO);
        }

        public ResponseDTO<LabDTO> DeleteLab(int id)
        {
            // Get the Lab to be Deleted
            LabDTO? labDTO = _labRepository.GetLabDTOById(id);
            if (labDTO == null)
                return new ResponseDTO<LabDTO>("Lab Not Found");
            // Delete the Lab
            if (!_labRepository.DeleteLab(id))
                return new ResponseDTO<LabDTO>("Failed to Delete Lab");
            return new ResponseDTO<LabDTO>(labDTO);
        }
    }
}
