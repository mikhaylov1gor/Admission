namespace DictionaryService.Infrastructure.Models.ExternalModels;


public class ExternalEducationLevelDto
{
    public int id { get; set; }
    public string name { get; set; }
}

public class ExternalEducationDocumentType
{
    public Guid id { get; set; }
    public DateTime createTime { get; set; }
    public string name { get; set; }
    public ExternalEducationLevelDto educationLevel { get; set; }
    public List<ExternalEducationLevelDto> nextEducationLevels { get; set; }
}

public class ExternalFaculty
{
    public Guid id { get; set; }
    public DateTime createTime { get; set; }
    public string name { get; set; }
}

public class ExternalEducationProgram
{
    public Guid id { get; set; }
    public DateTime createTime { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public string language { get; set; }
    public string educationForm { get; set; }
    public ExternalFaculty faculty { get; set; }
    public ExternalEducationLevelDto educationLevel { get; set; }
}

public class ExternalProgramsResponse
{
    public List<ExternalEducationProgram> programs { get; set; }
    public PaginationDto pagination { get; set; }
}

public class PaginationDto
{
    public int size { get; set; }
    public int count { get; set; }
    public int current { get; set; }
}