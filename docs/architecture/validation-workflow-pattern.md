# Validation Workflow Pattern

## Overview

This framework includes a **business-driven validation workflow** where validation rules are defined in **markdown** by business stakeholders, and **code is automatically generated** via GitHub Actions and GitHub Copilot agent integration.

## Vision

**Business stakeholders should own validation logic, not developers.**

Instead of:
```
Business → Requirements doc → Developer → Code → Testing → Deployment
```

We have:
```
Business → Edit markdown → GitHub Action → Copilot → PR → Review → Deployment
```

---

## Workflow Components

### 1. Validation Rules Markdown File

**Location**: `{Domain}/docs/{domain}-validation-rules.md`

**Example**: `Beneficiary/docs/beneficiary-validation-rules.md`

**Structure**:
```markdown
# Beneficiary Validation Rules

## Required Fields

### First Name
- **Rule**: Must be provided
- **Error Message**: "First name is required"
- **Severity**: Error

### Last Name
- **Rule**: Must be provided
- **Error Message**: "Last name is required"
- **Severity**: Error

### Date of Birth
- **Rule**: Must be provided
- **Format**: YYYY-MM-DD
- **Error Message**: "Date of birth is required and must be in YYYY-MM-DD format"
- **Severity**: Error

## Format Validations

### Email
- **Rule**: Must be valid email format if provided
- **Regex**: `^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`
- **Error Message**: "Email must be a valid email address"
- **Severity**: Warning

### Phone
- **Rule**: Must be valid phone format if provided
- **Regex**: `^\+?[1-9]\d{1,14}$`
- **Error Message**: "Phone must be a valid phone number"
- **Severity**: Warning

## Business Rules

### Age Requirement
- **Rule**: Beneficiary must be 18 years or older
- **Calculation**: Current year - birth year >= 18
- **Error Message**: "Beneficiary must be at least 18 years old"
- **Severity**: Error

### Duplicate Check
- **Rule**: Document number + nationality must be unique
- **Error Message**: "A beneficiary with this document number and nationality already exists"
- **Severity**: Error
```

### 2. GitHub Actions Workflow

**Location**: `.github/workflows/validation-rules-sync.yml`

```yaml
name: Validation Rules Sync

on:
  push:
    branches: [main]
    paths:
      - '**/docs/*-validation-rules.md'
  pull_request:
    paths:
      - '**/docs/*-validation-rules.md'

jobs:
  detect-changes:
    runs-on: ubuntu-latest
    outputs:
      changed-files: ${{ steps.changed-files.outputs.all_changed_files }}
    
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 2
      
      - name: Get changed files
        id: changed-files
        uses: tj-actions/changed-files@v41
        with:
          files: |
            **/*-validation-rules.md
      
      - name: List changed files
        run: |
          for file in ${{ steps.changed-files.outputs.all_changed_files }}; do
            echo "$file was changed"
          done
  
  create-issue:
    needs: detect-changes
    if: needs.detect-changes.outputs.changed-files != ''
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Get file diff
        id: get-diff
        run: |
          DIFF=$(git diff HEAD~1 HEAD -- ${{ needs.detect-changes.outputs.changed-files }})
          echo "diff<<EOF" >> $GITHUB_OUTPUT
          echo "$DIFF" >> $GITHUB_OUTPUT
          echo "EOF" >> $GITHUB_OUTPUT
      
      - name: Create Issue
        uses: actions/github-script@v7
        with:
          script: |
            const diff = `${{ steps.get-diff.outputs.diff }}`;
            const fileName = '${{ needs.detect-changes.outputs.changed-files }}';
            const domain = fileName.split('/')[0];
            
            const issue = await github.rest.issues.create({
              owner: context.repo.owner,
              repo: context.repo.repo,
              title: `Validation Rules Updated: ${domain}`,
              body: `## Validation Rules Changed
              
              **File**: \`${fileName}\`
              **Domain**: ${domain}
              
              ### Changes:
              \`\`\`diff
              ${diff}
              \`\`\`
              
              ### Next Steps:
              1. @github-copilot will analyze these changes
              2. Code will be generated across all layers:
                 - Domain validators
                 - API validation
                 - UI validation
                 - Test cases
              3. A pull request will be created for review
              
              ### Instructions for Copilot:
              Please generate code updates for the validation rule changes above. Follow the architecture patterns in Platform/docs/architecture/.
              
              Update the following files:
              - \`${domain}/src/Domain/Validators/{Entity}Validator.cs\`
              - \`${domain}/src/Api/Models/{Entity}Dto.cs\` (data annotations)
              - \`${domain}/src/UI/src/utils/validation.ts\`
              - \`${domain}/src/Test/{Entity}ValidationTests.cs\`
              `,
              labels: ['validation-sync', 'auto-generated', domain.toLowerCase()]
            });
            
            console.log(\`Created issue #\${issue.data.number}\`);
```

### 3. Copilot Agent Integration

**Instructions in Issue** (auto-generated by workflow):

```markdown
@github-copilot Please analyze the validation rule changes in this issue and generate code updates.

Follow these patterns:
1. Read `Platform/docs/architecture/requirements-to-architecture-mapping.md` for validation patterns
2. Update `{Domain}/src/Domain/Validators/{Entity}Validator.cs`
3. Update API DTOs with data annotations
4. Update UI validation (TypeScript)
5. Add/update test cases

Create a pull request with all changes.
```

**Copilot Agent Workflow**:
1. Reads architecture documentation
2. Analyzes validation rule changes (diff)
3. Generates validator class updates
4. Generates DTO attribute updates
5. Generates UI validation updates
6. Generates test case updates
7. Creates pull request
8. Tags reviewer for approval

---

## Code Generation Patterns

### Backend Validator (FluentValidation)

**Generated Code**:
```csharp
// Beneficiary/src/Domain/Validators/BeneficiaryValidator.cs
using FluentValidation;

public class BeneficiaryValidator : AbstractValidator<BeneficiaryDto>
{
    public BeneficiaryValidator()
    {
        // Required Fields
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required and must be in YYYY-MM-DD format")
            .Must(BeValidDate)
            .WithMessage("Date of birth must be a valid date");
        
        // Format Validations
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid email address")
            .WithSeverity(Severity.Warning);
        
        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone must be a valid phone number")
            .WithSeverity(Severity.Warning);
        
        // Business Rules
        RuleFor(x => x.DateOfBirth)
            .Must(BeAtLeast18YearsOld)
            .WithMessage("Beneficiary must be at least 18 years old");
    }
    
    private bool BeValidDate(string dateString)
    {
        return DateTime.TryParseExact(dateString, "yyyy-MM-dd", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
    
    private bool BeAtLeast18YearsOld(string dateOfBirthString)
    {
        if (!DateTime.TryParse(dateOfBirthString, out var dateOfBirth))
            return false;
            
        var age = DateTime.UtcNow.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.UtcNow.AddYears(-age))
            age--;
            
        return age >= 18;
    }
}
```

### API DTO Annotations

**Generated Code**:
```csharp
// Beneficiary/src/Api/Models/BeneficiaryRegistrationDto.cs
using System.ComponentModel.DataAnnotations;

public class BeneficiaryRegistrationDto
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Date of birth is required and must be in YYYY-MM-DD format")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", 
        ErrorMessage = "Date of birth must be in YYYY-MM-DD format")]
    public string DateOfBirth { get; set; }
    
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; }
    
    [Phone(ErrorMessage = "Phone must be a valid phone number")]
    public string Phone { get; set; }
    
    [Required]
    public string Nationality { get; set; }
    
    [Required]
    public string DocumentType { get; set; }
    
    [Required]
    public string DocumentNumber { get; set; }
}
```

### UI Validation (TypeScript)

**Generated Code**:
```typescript
// Beneficiary/src/UI/src/utils/validation.ts
import { BeneficiaryDto } from '../types/beneficiary.types';

export interface ValidationError {
  field: string;
  message: string;
  severity: 'error' | 'warning';
}

export const validateBeneficiary = (data: BeneficiaryDto): ValidationError[] => {
  const errors: ValidationError[] = [];
  
  // Required Fields
  if (!data.firstName || data.firstName.trim() === '') {
    errors.push({
      field: 'firstName',
      message: 'First name is required',
      severity: 'error'
    });
  }
  
  if (!data.lastName || data.lastName.trim() === '') {
    errors.push({
      field: 'lastName',
      message: 'Last name is required',
      severity: 'error'
    });
  }
  
  if (!data.dateOfBirth) {
    errors.push({
      field: 'dateOfBirth',
      message: 'Date of birth is required and must be in YYYY-MM-DD format',
      severity: 'error'
    });
  }
  
  // Format Validations
  if (data.email && data.email.trim() !== '') {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(data.email)) {
      errors.push({
        field: 'email',
        message: 'Email must be a valid email address',
        severity: 'warning'
      });
    }
  }
  
  if (data.phone && data.phone.trim() !== '') {
    const phoneRegex = /^\+?[1-9]\d{1,14}$/;
    if (!phoneRegex.test(data.phone)) {
      errors.push({
        field: 'phone',
        message: 'Phone must be a valid phone number',
        severity: 'warning'
      });
    }
  }
  
  // Business Rules
  if (data.dateOfBirth) {
    const birthDate = new Date(data.dateOfBirth);
    const today = new Date();
    const age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    const actualAge = monthDiff < 0 || 
      (monthDiff === 0 && today.getDate() < birthDate.getDate())
      ? age - 1
      : age;
    
    if (actualAge < 18) {
      errors.push({
        field: 'dateOfBirth',
        message: 'Beneficiary must be at least 18 years old',
        severity: 'error'
      });
    }
  }
  
  return errors;
};

export const hasErrors = (errors: ValidationError[]): boolean => {
  return errors.some(e => e.severity === 'error');
};

export const hasWarnings = (errors: ValidationError[]): boolean => {
  return errors.some(e => e.severity === 'warning');
};
```

### Test Cases

**Generated Code**:
```csharp
// Beneficiary/src/Test/BeneficiaryValidationTests.cs
using Xunit;
using FluentValidation.TestHelper;

public class BeneficiaryValidationTests
{
    private readonly BeneficiaryValidator _validator;
    
    public BeneficiaryValidationTests()
    {
        _validator = new BeneficiaryValidator();
    }
    
    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var dto = new BeneficiaryRegistrationDto { FirstName = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }
    
    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var dto = new BeneficiaryRegistrationDto { LastName = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }
    
    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Should_Have_Warning_When_Email_Is_Invalid(string email)
    {
        var dto = new BeneficiaryRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1990-01-01",
            Email = email 
        };
        
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address");
    }
    
    [Fact]
    public void Should_Have_Error_When_Age_Under_18()
    {
        var dto = new BeneficiaryRegistrationDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-17).ToString("yyyy-MM-dd")
        };
        
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Beneficiary must be at least 18 years old");
    }
    
    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new BeneficiaryRegistrationDto
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1990-01-01",
            Nationality = "USA",
            DocumentType = "PASSPORT",
            DocumentNumber = "123456789",
            Email = "john.doe@example.com",
            Phone = "+1234567890"
        };
        
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
```

---

## Validation Rule Template

### Rule Structure

```markdown
### {Field Name}
- **Rule**: {Description of validation rule}
- **Format**: {Expected format, if applicable}
- **Regex**: {Regex pattern, if applicable}
- **Calculation**: {Formula, if business rule}
- **Error Message**: "{User-friendly error message}"
- **Severity**: Error | Warning
- **Dependencies**: {Other fields this rule depends on}
```

### Example: Complex Business Rule

```markdown
### Case Closure Date
- **Rule**: Case closure date must be after registration date
- **Calculation**: closureDate > registrationDate
- **Error Message**: "Case closure date must be after registration date"
- **Severity**: Error
- **Dependencies**: registrationDate, closureDate
```

**Generated Code**:
```csharp
RuleFor(x => x)
    .Must(x => x.CaseClosureDate > x.RegistrationDate)
    .When(x => x.CaseClosureDate.HasValue)
    .WithMessage("Case closure date must be after registration date");
```

---

## Developer Workflow

### Scenario: Business Adds New Validation Rule

**Step 1**: Business stakeholder edits markdown file

```markdown
### Social Security Number
- **Rule**: Must be 9 digits if provided
- **Regex**: `^\d{9}$`
- **Error Message**: "Social Security Number must be exactly 9 digits"
- **Severity**: Warning
```

**Step 2**: Commit and push to repository

```bash
git add Beneficiary/docs/beneficiary-validation-rules.md
git commit -m "Add SSN validation rule"
git push origin main
```

**Step 3**: GitHub Action detects change

- Workflow runs automatically
- Creates GitHub issue with diff
- Tags @github-copilot

**Step 4**: Copilot agent analyzes and generates code

- Reads architecture docs
- Parses markdown changes
- Generates validator update
- Generates DTO update
- Generates UI validation update
- Generates test cases
- Creates pull request

**Step 5**: Developer reviews PR

- Verify code quality
- Run tests
- Check for edge cases
- Approve and merge

**Step 6**: Deploy

- Automated deployment pipeline
- New validation rules active

---

## Benefits

### For Business Stakeholders
- **Direct control**: Edit rules without developer
- **Clear documentation**: Rules in plain English
- **Fast iteration**: Change rules quickly
- **Transparency**: See exactly what rules are active

### For Developers
- **Less manual work**: No hand-coding validation logic
- **Consistency**: Same patterns everywhere
- **Test coverage**: Tests auto-generated
- **Documentation sync**: Rules always match code

### For QA
- **Clear requirements**: Validation rules documented
- **Test cases generated**: Less manual test writing
- **Traceability**: Link rules to code changes

---

## Advanced Patterns

### Cross-Field Validation

**Markdown**:
```markdown
### End Date vs Start Date
- **Rule**: End date must be after start date
- **Calculation**: endDate > startDate
- **Error Message**: "End date must be after start date"
- **Severity**: Error
- **Dependencies**: startDate, endDate
```

**Generated Code**:
```csharp
RuleFor(x => x.EndDate)
    .GreaterThan(x => x.StartDate)
    .When(x => x.EndDate.HasValue && x.StartDate.HasValue)
    .WithMessage("End date must be after start date");
```

### Conditional Validation

**Markdown**:
```markdown
### Passport Number
- **Rule**: Required if document type is "PASSPORT"
- **Condition**: documentType == "PASSPORT"
- **Error Message**: "Passport number is required when document type is PASSPORT"
- **Severity**: Error
- **Dependencies**: documentType
```

**Generated Code**:
```csharp
RuleFor(x => x.PassportNumber)
    .NotEmpty()
    .When(x => x.DocumentType == "PASSPORT")
    .WithMessage("Passport number is required when document type is PASSPORT");
```

### Database Validation

**Markdown**:
```markdown
### Document Number Uniqueness
- **Rule**: Document number + nationality must be unique in database
- **Query**: SELECT COUNT(*) FROM Beneficiaries WHERE DocumentNumber = @docNum AND Nationality = @nat
- **Error Message**: "A beneficiary with this document number and nationality already exists"
- **Severity**: Error
- **Type**: Async
```

**Generated Code**:
```csharp
RuleFor(x => x)
    .MustAsync(async (dto, cancellation) =>
    {
        var existing = await _repository.FindByDocumentAsync(
            dto.DocumentNumber, 
            dto.Nationality);
        return existing == null;
    })
    .WithMessage("A beneficiary with this document number and nationality already exists");
```

---

## Best Practices

### 1. Markdown Clarity
```markdown
✅ Good:
### Email
- **Rule**: Must be valid email format if provided
- **Regex**: `^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`
- **Error Message**: "Email must be a valid email address"
- **Severity**: Warning

❌ Bad:
### Email
- Must be valid
```

### 2. Error Messages
```markdown
✅ Good: "Date of birth must be in YYYY-MM-DD format"
❌ Bad: "Invalid date"

✅ Good: "Beneficiary must be at least 18 years old"
❌ Bad: "Age error"
```

### 3. Severity Levels
- **Error**: Blocks submission (required fields, data integrity)
- **Warning**: Allows submission with confirmation (format issues, recommendations)

### 4. Testing
```csharp
// Test both valid and invalid cases
[Theory]
[InlineData("john.doe@example.com", true)]   // Valid
[InlineData("invalid-email", false)]         // Invalid
[InlineData("", true)]                       // Empty allowed
public void Email_Validation_Tests(string email, bool expected)
{
    var dto = CreateValidDto();
    dto.Email = email;
    var result = _validator.Validate(dto);
    Assert.Equal(expected, result.IsValid);
}
```

---

**Next**: See [Bulk Import Pattern](bulk-import-pattern.md) for complete CSV/Excel upload flow with validation, sagas, and SignalR progress tracking.
