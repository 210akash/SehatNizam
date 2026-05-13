using ERP.Entities.Models;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.LabOrderType.Command
{
    public class SaveLabTestVariableCommand : IRequest<int>
    {
        public long LabOrderTypeId { get; set; }

        public List<LabTestVariableDto> Variables { get; set; }
            = new List<LabTestVariableDto>();
    }

    public class LabTestVariableDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        // Numeric ranges
        public decimal? MaleMin { get; set; }

        public decimal? MaleMax { get; set; }

        public decimal? FemaleMin { get; set; }

        public decimal? FemaleMax { get; set; }

        public bool HasGenderRange { get; set; }

        public int DisplayOrder { get; set; }

        // Enum Type
        public ResultType ResultType { get; set; }

        // Optional flags
        public bool IsRequired { get; set; }

        public bool IsCalculated { get; set; }

        // For qualitative tests
        public List<LabTestVariableOptionDto> Options { get; set; }
            = new List<LabTestVariableOptionDto>();
    }

    public class LabTestVariableOptionDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }
    }

    //public class SaveLabTestVariableCommand : IRequest<int>
    //{
    //    public long LabOrderTypeId { get; set; }
    //    public List<LabTestVariableDto> Variables { get; set; }
    //}

    //public class LabTestVariableDto
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public string Unit { get; set; }
    //    public decimal? MaleMin { get; set; }
    //    public decimal? MaleMax { get; set; }
    //    public decimal? FemaleMin { get; set; }
    //    public decimal? FemaleMax { get; set; }
    //    public bool HasGenderRange { get; set; }
    //    public int DisplayOrder { get; set; }
    //    public int ResultTypeId { get; set; }
    //}

    //public class LabTestVariableOptionDto
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public int DisplayOrder { get; set; }
    //}
}
