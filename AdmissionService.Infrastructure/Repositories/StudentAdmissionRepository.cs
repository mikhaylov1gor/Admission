using AdmissionService.Domain.Entities;
using AdmissionService.Domain.IRepositories;
using AdmissionService.Infrastructure.Persistence;
using Contract.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Repositories;

public class StudentAdmissionRepository : IStudentAdmissionRepository
{
    private readonly AdmissionDbContext _context;
    public StudentAdmissionRepository(AdmissionDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsStudentAdmissionExists(Guid applicantId)
    {
        var admissionSeasonId = _context.AdmissionSettings.First().AdmissionSeasonId;
        var isExists = await _context.StudentAdmissions
            .AnyAsync(sa => sa.ApplicantId == applicantId && sa.AdmissionSeasonId == admissionSeasonId);
    
        return isExists;
    }

    public async Task AddStudentAdmissionAsync(StudentAdmission studentAdmission)
    {
        await _context.StudentAdmissions.AddAsync(studentAdmission);
        await _context.SaveChangesAsync();
    }

    public async Task<StudentAdmission?> GetCurrentStudentAdmissionAsync(Guid applicantId)
    {
        var admissionSeasonId = _context.AdmissionSettings.First().AdmissionSeasonId;
        var studentAdmission = await _context.StudentAdmissions
            .Include(sa => sa.AdmissionPrograms)
            .FirstOrDefaultAsync(sa => sa.ApplicantId == applicantId && sa.AdmissionSeasonId == admissionSeasonId);
        
        return studentAdmission;
    }

    public async Task<StudentAdmission?> GetStudentAdmissionByIdAsync(Guid studentAdmissionId, Guid userId)
    {
        return await _context.StudentAdmissions
            .Include(sa => sa.AdmissionPrograms)
            .FirstOrDefaultAsync(sa => sa.ApplicantId == userId && sa.Id == studentAdmissionId);
    }

    public async Task DeleteStudentAdmissionProgramByIdAsync(Guid studentAdmissionId, Guid programId)
    {
        var admission = await _context.StudentAdmissions
            .Include(sa => sa.AdmissionPrograms)
            .FirstOrDefaultAsync(sa => sa.Id == studentAdmissionId);
        
        var programToDelete = admission.AdmissionPrograms
            .FirstOrDefault(ap => ap.Id == programId);

        admission.AdmissionPrograms.Remove(programToDelete);
        _context.AdmissionPrograms.Remove(programToDelete); 

        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAdmissionProgram(AdmissionProgram program)
    {
        var existingProgram = await _context.AdmissionPrograms.FindAsync(program.Id);
        if (existingProgram != null)
        {
            existingProgram.Priority = program.Priority;
            existingProgram.ModifiedTime = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<StudentAdmission>?> GetStudentAdmissions(Guid userId)
    {
        return await _context.StudentAdmissions
            .Include(sa => sa.AdmissionPrograms)
            .Where(sa => sa.ApplicantId == userId)
            .ToListAsync();
    }
    
    public async Task AddAdmissionProgramAsync(AdmissionProgram program)
    {
        await _context.AdmissionPrograms.AddAsync(program);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}