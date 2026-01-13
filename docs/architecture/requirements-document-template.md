# Requirements Document Template

## Overview

This template provides a **standardized format** for business requirements that **map directly to architectural components**. When requirements follow this structure, GitHub Copilot can generate complete domain implementations automatically.

## Template Structure

```markdown
# {Domain Name} Requirements

## 1. Domain Overview

**Purpose**: {What business problem does this domain solve?}

**Scope**: {What is included and what is excluded?}

**Stakeholders**: {Who are the primary users and decision makers?}

## 2. Entities

### {Entity Name}

**Description**: {What this entity represents}

**Properties**:
- **{PropertyName}** ({Type}): {Description} [Required/Optional]
- **{PropertyName}** ({Type}): {Description} [Required/Optional]

**Business Rules**:
- {Rule description}
- {Rule description}

**Example**:
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "propertyName": "value"
}
```

## 3. Operations

### {Operation Name}

**Description**: {What this operation does}

**Trigger**: {User action / Event / Schedule}

**Input**:
- **{ParameterName}** ({Type}): {Description}

**Output**:
- **{ResultName}** ({Type}): {Description}

**Business Logic**:
1. {Step description}
2. {Step description}
3. {Step description}

**Validation Rules**:
- {Rule}: {Error message}
- {Rule}: {Error message}

**Success Criteria**:
- {Criterion}
- {Criterion}

**Error Cases**:
- {Error condition}: {How to handle}

**Events Published**:
- **{EventName}**: When {condition}

## 4. Workflows

### {Workflow Name}

**Description**: {Multi-step process description}

**Steps**:
1. **{StepName}**: {Action} → {Result}
2. **{StepName}**: {Action} → {Result}
3. **{StepName}**: {Action} → {Result}

**Compensation Logic** (if failure):
- {Rollback action}

**Timeout**: {Duration}

## 5. Validation Rules

### {Entity/Field Name}

**Required Fields**:
- {FieldName}: "{Error message}"

**Format Validations**:
- {FieldName}: {Format/Regex} → "{Error message}"

**Business Rules**:
- {Rule description}: "{Error message}"

**Cross-Field Validations**:
- {Condition}: "{Error message}"

## 6. User Interface

### {Screen/Page Name}

**Purpose**: {What user accomplishes here}

**Layout**:
- {Section}: {Content}
- {Section}: {Content}

**Actions**:
- **{ButtonName}**: {What it does}

**Validations**:
- {Field}: {Validation rule}

**Mock-up**: [Optional wireframe/screenshot]

## 7. Integrations

### {External System Name}

**Purpose**: {Why we integrate}

**Data Flow**: {Our system} → {External system} OR {External system} → {Our system}

**Events**:
- **Outbound**: {EventName} when {condition}
- **Inbound**: {EventName} triggers {action}

**Error Handling**:
- {Error case}: {How to handle}

## 8. Reporting & Queries

### {Report/Query Name}

**Purpose**: {What question this answers}

**Inputs**:
- {Filter}: {Options}

**Output Fields**:
- {FieldName}: {Description}

**Performance**: {Expected volume, response time}

## 9. Security & Permissions

**Who can**:
- **Create**: {Roles}
- **Read**: {Roles}
- **Update**: {Roles}
- **Delete**: {Roles}

**Data Sensitivity**: {Classification level}

## 10. Acceptance Criteria

- [ ] {Testable criterion}
- [ ] {Testable criterion}
- [ ] {Testable criterion}
```

---

## Complete Example: Questions Domain

```markdown
# Questions Domain Requirements

## 1. Domain Overview

**Purpose**: Enable creation, management, and answering of multiple-choice questions for training and assessment.

**Scope**:
- Creating questions with multiple choice answers
- Tracking user answers
- Scoring and awarding points for correct answers
- **Out of scope**: Essay questions, file uploads, automated grading

**Stakeholders**:
- Training coordinators (create questions)
- Beneficiaries (answer questions)
- Program managers (view results)

## 2. Entities

### Question

**Description**: A multiple-choice question with one correct answer

**Properties**:
- **id** (Guid): Unique identifier [System-generated]
- **text** (string): Question text [Required, 10-500 chars]
- **category** (string): Topic category [Required, from predefined list]
- **difficulty** (enum): EASY | MEDIUM | HARD [Required]
- **points** (int): Points awarded for correct answer [Required, 1-100]
- **createdBy** (Guid): User who created question [System-generated]
- **createdAt** (DateTime): Creation timestamp [System-generated]
- **isActive** (bool): Whether question is currently available [Default: true]

**Business Rules**:
- Question must have exactly 4 answer options
- Exactly one answer must be marked as correct
- Question text must be unique within category
- Cannot modify question after 10 answers received

**Example**:
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "text": "What is the capital of France?",
  "category": "Geography",
  "difficulty": "EASY",
  "points": 10,
  "createdBy": "user-123",
  "createdAt": "2024-01-15T10:30:00Z",
  "isActive": true
}
```

### Answer Option

**Description**: One possible answer to a question

**Properties**:
- **id** (Guid): Unique identifier
- **questionId** (Guid): Parent question [Required]
- **text** (string): Answer text [Required, 1-200 chars]
- **isCorrect** (bool): Whether this is the correct answer [Required]
- **displayOrder** (int): Order to display (1-4) [Required]

### User Answer

**Description**: Record of a user's answer to a question

**Properties**:
- **id** (Guid): Unique identifier
- **userId** (Guid): User who answered [Required]
- **questionId** (Guid): Question answered [Required]
- **answerOptionId** (Guid): Answer selected [Required]
- **isCorrect** (bool): Whether answer was correct [Calculated]
- **pointsAwarded** (int): Points received [Calculated]
- **answeredAt** (DateTime): When answered [System-generated]

## 3. Operations

### Create Question

**Description**: Create new multiple-choice question

**Trigger**: Training coordinator clicks "Create Question" button

**Input**:
- **text** (string): Question text
- **category** (string): Category selection
- **difficulty** (enum): Difficulty level
- **points** (int): Points for correct answer
- **answerOptions** (array): 4 answer options with text and isCorrect flag

**Output**:
- **questionId** (Guid): ID of created question

**Business Logic**:
1. Validate all required fields present
2. Check question text is unique in category
3. Verify exactly 4 answer options provided
4. Verify exactly one answer marked as correct
5. Generate unique ID for question
6. Save question to repository
7. Publish QuestionCreatedEvent

**Validation Rules**:
- Text: 10-500 characters → "Question must be between 10 and 500 characters"
- Category: Must be from predefined list → "Invalid category selected"
- Points: 1-100 → "Points must be between 1 and 100"
- Answer Options: Exactly 4 → "Must provide exactly 4 answer options"
- Correct Answer: Exactly 1 → "Must mark exactly one answer as correct"

**Success Criteria**:
- Question saved to database
- QuestionCreatedEvent published
- Question appears in active questions list
- Training coordinator receives confirmation

**Error Cases**:
- Duplicate question text → Show error: "This question already exists in this category"
- Database failure → Retry 3 times, then show: "Unable to save question. Please try again."

**Events Published**:
- **QuestionCreatedEvent**: When question successfully saved

### Answer Question

**Description**: User submits answer to question

**Trigger**: Beneficiary clicks "Submit Answer" button

**Input**:
- **questionId** (Guid): Question being answered
- **answerOptionId** (Guid): Selected answer

**Output**:
- **isCorrect** (bool): Whether answer was correct
- **pointsAwarded** (int): Points received
- **correctAnswerText** (string): Text of correct answer (if wrong)

**Business Logic**:
1. Check user hasn't already answered this question
2. Validate question and answer option exist
3. Determine if answer is correct
4. Calculate points (full points if correct, 0 if wrong)
5. Save user answer to repository
6. If correct: Publish CorrectAnswerGivenEvent (for Points domain)
7. If wrong: Return correct answer for learning

**Validation Rules**:
- Question exists → "Question not found"
- Answer option belongs to question → "Invalid answer option"
- User hasn't answered before → "You have already answered this question"

**Success Criteria**:
- Answer recorded in database
- Correct/incorrect feedback shown immediately
- If correct: Points awarded event published
- If wrong: Correct answer shown

**Events Published**:
- **QuestionAnsweredEvent**: Always published
- **CorrectAnswerGivenEvent**: When answer is correct (consumed by Points domain)

## 4. Workflows

### Question Creation Workflow

**Description**: End-to-end process for creating and activating a question

**Steps**:
1. **Draft Creation**: User fills out question form → Question saved as draft (isActive=false)
2. **Peer Review** (optional): Another coordinator reviews → Approved or rejected
3. **Activation**: Coordinator activates question → isActive=true, QuestionActivatedEvent published
4. **Availability**: Question appears in active questions pool

**Compensation Logic** (if activation fails):
- Revert isActive to false
- Notify coordinator of failure

**Timeout**: 30 seconds for each step

## 5. Validation Rules

### Question Entity

**Required Fields**:
- text: "Question text is required"
- category: "Category is required"
- difficulty: "Difficulty level is required"
- points: "Points value is required"

**Format Validations**:
- text: 10-500 characters → "Question must be between 10 and 500 characters"
- points: Integer, 1-100 → "Points must be between 1 and 100"

**Business Rules**:
- Unique text per category → "This question already exists in this category"
- Exactly 4 answer options → "Must provide exactly 4 answer options"
- Exactly 1 correct answer → "Must mark exactly one answer as correct"

**Cross-Field Validations**:
- If difficulty=HARD, points >= 20 → "Hard questions must award at least 20 points"

## 6. User Interface

### Create Question Page

**Purpose**: Enable training coordinators to create new questions

**Layout**:
- **Header**: "Create New Question" title + Save/Cancel buttons
- **Question Section**: Text area for question (10-500 chars, live counter)
- **Details Section**: Category dropdown, Difficulty radio buttons, Points number input
- **Answers Section**: 4 text inputs with radio button to mark correct answer
- **Preview Section**: Shows how question will appear to users

**Actions**:
- **Save**: Validates and creates question
- **Cancel**: Returns to questions list (confirms if unsaved changes)
- **Preview**: Shows question in user view

**Validations**:
- Show inline error messages below each field
- Disable Save button until all validations pass
- Show character count for question text (500/500)

### Answer Question Page

**Purpose**: Allow users to view and answer questions

**Layout**:
- **Question Card**: Question text, category badge, difficulty badge, points value
- **Answer Options**: 4 radio buttons with answer text
- **Submit Button**: Disabled until option selected
- **Feedback Area**: Shows correct/incorrect after submission

**Actions**:
- **Submit Answer**: Submits selected answer
- **Next Question**: Loads next unanswered question (after answering)

**Validations**:
- Must select an answer before submitting
- Cannot change answer after submission

## 7. Integrations

### Points Domain

**Purpose**: Award points for correct answers

**Data Flow**: Questions → Points (one-way)

**Events**:
- **Outbound**: CorrectAnswerGivenEvent when user answers correctly
  - Payload: { userId, questionId, pointsToAward, answeredAt }
- **Inbound**: None (Points domain doesn't send events back)

**Error Handling**:
- If Points domain fails to award points: Log error, show warning to user ("Points will be awarded later")

### Audit Domain

**Purpose**: Track all question creation and answer activity

**Data Flow**: Questions → Audit (one-way)

**Events**:
- **Outbound**: 
  - QuestionCreatedEvent
  - QuestionAnsweredEvent
  - QuestionActivatedEvent

## 8. Reporting & Queries

### Question Performance Report

**Purpose**: Show which questions are most challenging

**Inputs**:
- **Category** (optional): Filter by category
- **Date Range**: From/To dates

**Output Fields**:
- Question Text
- Category
- Difficulty
- Total Answers
- Correct Answers
- Incorrect Answers
- Success Rate (%)
- Average Points Awarded

**Performance**: Support up to 10,000 questions, 100,000 answers. Response time < 2 seconds.

### User Answer History

**Purpose**: Show all questions answered by a specific user

**Inputs**:
- **userId**: User to query

**Output Fields**:
- Question Text
- Category
- User's Answer
- Correct Answer
- Points Awarded
- Answered Date

## 9. Security & Permissions

**Who can**:
- **Create Question**: Training Coordinator, Admin
- **Read Questions**: All authenticated users
- **Update Question**: Training Coordinator (owner), Admin (if < 10 answers)
- **Delete Question**: Admin only (if 0 answers)
- **Answer Question**: Beneficiary, Admin

**Data Sensitivity**: Internal Use Only

## 10. Acceptance Criteria

- [ ] Training coordinator can create question with 4 answers
- [ ] Exactly one answer must be marked correct (enforced)
- [ ] User can submit answer to active question
- [ ] Correct answer awards points immediately
- [ ] Incorrect answer shows correct answer
- [ ] User cannot answer same question twice
- [ ] Question with >10 answers cannot be modified
- [ ] All events published correctly
- [ ] Real-time feedback shown via SignalR
- [ ] Performance report loads in < 2 seconds
```

---

## Mapping Requirements to Architecture

Once requirements follow this template, GitHub Copilot can generate:

### From **Entities** Section →
- **Domain Models**: `Question.cs`, `AnswerOption.cs`, `UserAnswer.cs`
- **DTOs**: `QuestionDto.cs`, `CreateQuestionDto.cs`
- **Document Models**: `QuestionDocument.cs` (for CosmosDB)
- **Repositories**: `IQuestionRepository.cs`, `QuestionRepository.cs`

### From **Operations** Section →
- **Commands**: `CreateQuestionCommand.cs`, `AnswerQuestionCommand.cs`
- **Command Handlers**: `CreateQuestionCommandHandler.cs`
- **API Endpoints**: `CreateQuestionFunction.cs`

### From **Workflows** Section →
- **Sagas**: `QuestionCreationSaga.cs` with saga data and handlers
- **Events**: `QuestionCreatedEvent.cs`, `QuestionActivatedEvent.cs`

### From **Validation Rules** Section →
- **Validators**: `QuestionValidator.cs` (FluentValidation)
- **UI Validation**: `validation.ts` (TypeScript)
- **Test Cases**: `QuestionValidationTests.cs`

### From **User Interface** Section →
- **React Pages**: `CreateQuestionPage.tsx`, `AnswerQuestionPage.tsx`
- **React Components**: `QuestionCard.tsx`, `AnswerOptions.tsx`

### From **Integrations** Section →
- **Event Handlers**: `CorrectAnswerGivenEventHandler.cs` (in Points domain)
- **SignalR Handlers**: `SignalRQuestionHandler.cs` (in Platform domain)

### From **Reporting** Section →
- **Query Models**: `QuestionPerformanceDto.cs`
- **Query Handlers**: `GetQuestionPerformanceQueryHandler.cs`
- **API Endpoints**: `QuestionReportsFunction.cs`

---

## Best Practices

### 1. Be Specific
```markdown
✅ Good: "Question text must be between 10 and 500 characters"
❌ Bad: "Question should be a reasonable length"
```

### 2. Include Examples
```markdown
✅ Good: Provide JSON example of entity with realistic data
❌ Bad: Skip examples
```

### 3. Define Error Messages
```markdown
✅ Good: "Must provide exactly 4 answer options"
❌ Bad: "Invalid answers"
```

### 4. Specify Events
```markdown
✅ Good: **Events Published**: CorrectAnswerGivenEvent when answer is correct
❌ Bad: "Something happens when user answers"
```

---

**Next**: See [Event Storming Guide](event-storming-guide.md) for discovering domains, events, and workflows collaboratively.
