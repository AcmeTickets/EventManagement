# Event Storming Guide

## Overview

Event Storming is a **collaborative workshop** technique for discovering domain events, commands, aggregates, and bounded contexts. It's ideal for understanding business processes and designing event-driven architectures.

## What is Event Storming?

**Event Storming** = Group workshop where domain experts and developers discover the business domain by mapping out **events** that happen over time.

**Why Event Storming?**
- **Fast**: Discover entire domains in hours, not weeks
- **Collaborative**: Business and tech work together
- **Visual**: Physical or virtual sticky notes
- **Event-driven first**: Naturally leads to event-driven architecture

---

## Workshop Format

### Participants
- **Domain Experts**: Business stakeholders who know the process
- **Developers**: Technical team building the system
- **Facilitator**: Guides the workshop (ideally experienced in Event Storming)
- **Optional**: UX designers, QA, product managers

### Duration
- **Small domain**: 2-4 hours
- **Medium domain**: 1 day
- **Large/complex domain**: 2-3 days

### Materials
- **Physical**: Large wall space, sticky notes (multiple colors), markers
- **Virtual**: Miro, Mural, or similar online whiteboard tools

---

## Color-Coded Sticky Notes

| Color | Represents | Example |
|-------|------------|---------|
| **Orange** | Domain Events (past tense) | "BeneficiaryRegistered", "QuestionAnswered" |
| **Blue** | Commands (imperative) | "RegisterBeneficiary", "AnswerQuestion" |
| **Yellow** | Actors/Users | "Training Coordinator", "Beneficiary" |
| **Pink** | External Systems | "Payment Gateway", "Email Service" |
| **Purple** | Policies/Business Rules | "When correct answer given, award points" |
| **Green** | Read Models/Views | "Beneficiary List", "Question Performance Dashboard" |
| **Red** | Issues/Questions | "What happens if duplicate?" |
| **Large Purple** | Aggregates/Bounded Contexts | "Beneficiary", "Questions", "Points" |

---

## Event Storming Process

### Phase 1: Chaotic Exploration (30-60 min)

**Goal**: Brain dump all events that happen in the domain

**Steps**:
1. Facilitator explains: "Write down everything that happens in this business process"
2. Participants write **domain events** (orange sticky notes) in **past tense**
3. **No order**, **no judgment** - just get everything out
4. Place events anywhere on the wall
5. Duplicates are OK (will clean up later)

**Examples**:
- "BeneficiaryRegistered"
- "DocumentUploaded"
- "QuestionAnswered"
- "PointsAwarded"
- "CaseClosed"

**Facilitator Tips**:
- Encourage participation from everyone
- Ask: "What else happens?"
- Remind: Past tense, business language
- Don't worry about order yet

### Phase 2: Timeline Ordering (30-45 min)

**Goal**: Arrange events in chronological order

**Steps**:
1. Group events into logical flows
2. Arrange left to right (time flows left → right)
3. Identify which events trigger others
4. Find **parallel processes** (events happening at same time)
5. Remove duplicates

**Example Flow**:
```
BeneficiaryRegistered → DocumentUploaded → DocumentVerified → CaseActivated → 
QuestionAssigned → QuestionAnswered → CorrectAnswerGiven → PointsAwarded
```

**Look For**:
- **Sequences**: Event A → Event B → Event C
- **Branches**: Event A → (Event B OR Event C)
- **Loops**: Event A → Event B → Event A (retry scenarios)

### Phase 3: Add Commands (20-30 min)

**Goal**: Identify what **triggers** each event

**Steps**:
1. For each event, ask: "What action caused this?"
2. Add **blue command** sticky note before event
3. Commands are **imperative** (verb form)
4. One command can trigger multiple events

**Example**:
```
[RegisterBeneficiary] → BeneficiaryRegistered
[AnswerQuestion] → QuestionAnswered → (if correct) → CorrectAnswerGiven
[UploadDocument] → DocumentUploaded
```

**Pattern**:
```
[Command] → Event
[Command] → Event1 → Event2 (cascade)
[Command] → (Event1 OR Event2) (conditional)
```

### Phase 4: Add Actors (15-20 min)

**Goal**: Identify **who** executes each command

**Steps**:
1. For each command, add **yellow actor** sticky note
2. Actors can be:
   - Users (Beneficiary, Training Coordinator)
   - External systems (Payment Gateway)
   - Time-based triggers (Daily Job)
3. Place actor **above** the command

**Example**:
```
    [Training Coordinator]
           ↓
    [CreateQuestion] → QuestionCreated
    
    [Beneficiary]
           ↓
    [AnswerQuestion] → QuestionAnswered
```

### Phase 5: Add Policies (20-30 min)

**Goal**: Discover **business rules** and **automation**

**Steps**:
1. Look for patterns: "When Event X happens, then do Y"
2. Add **purple policy** sticky note
3. Policies often trigger commands automatically

**Example**:
```
QuestionAnswered → [Policy: If correct, award points] → [AwardPoints] → PointsAwarded
```

**Common Policies**:
- "When beneficiary registered, send welcome email"
- "When document uploaded, start verification process"
- "When 3 failed login attempts, lock account"
- "When case inactive for 30 days, send reminder"

### Phase 6: Identify Aggregates (30-45 min)

**Goal**: Group related events/commands into **aggregates**

**Steps**:
1. Look for clusters of events around same concept
2. Draw **large purple box** around cluster
3. Name aggregate (singular noun): "Beneficiary", "Question", "Case"
4. Aggregates are consistency boundaries

**Example Clusters**:

**Beneficiary Aggregate**:
```
[RegisterBeneficiary] → BeneficiaryRegistered
[UpdateBeneficiaryStatus] → BeneficiaryStatusUpdated
[CloseCase] → CaseClosed
```

**Question Aggregate**:
```
[CreateQuestion] → QuestionCreated
[ActivateQuestion] → QuestionActivated
[AnswerQuestion] → QuestionAnswered
```

**Rules for Aggregates**:
- One aggregate = one consistency boundary
- Commands targeting aggregate enforce business rules
- Events from aggregate notify other aggregates

### Phase 7: Identify Bounded Contexts (45-60 min)

**Goal**: Discover **domain boundaries** (separate microservices/modules)

**Steps**:
1. Look for **natural separations** between aggregates
2. Ask: "Could this aggregate exist independently?"
3. Identify **context boundaries** (dashed lines on board)
4. Name each bounded context (domain name)

**Example Bounded Contexts**:

```
┌─────────────────────────┐
│ Beneficiary Context     │
│ - Beneficiary aggregate │
│ - Case aggregate        │
└─────────────────────────┘

┌─────────────────────────┐
│ Questions Context       │
│ - Question aggregate    │
│ - Answer aggregate      │
└─────────────────────────┘

┌─────────────────────────┐
│ Points Context          │
│ - PointsAccount agg.    │
│ - Transaction agg.      │
└─────────────────────────┘
```

**Characteristics of Bounded Context**:
- Has its own **database**
- Has its own **team ownership**
- Communicates via **events** (not direct calls)
- Can evolve **independently**

---

## Example: Event Storming Session

### Scenario: Beneficiary Management Process

**Phase 1: Chaotic Exploration**

Participants write events (no order):
- BeneficiaryRegistered
- DocumentUploaded
- DocumentVerified
- DocumentRejected
- CaseActivated
- StatusUpdated
- InterviewScheduled
- InterviewCompleted
- CaseClosed
- EmailSent
- NotificationSent

**Phase 2: Timeline Ordering**

Arrange chronologically:
```
BeneficiaryRegistered → DocumentUploaded → DocumentVerified → CaseActivated →
InterviewScheduled → InterviewCompleted → CaseClosed

                                        ↓ (if rejected)
                                  DocumentRejected → EmailSent
```

**Phase 3: Add Commands**

```
[RegisterBeneficiary] → BeneficiaryRegistered
[UploadDocument] → DocumentUploaded
[VerifyDocument] → DocumentVerified (or DocumentRejected)
[ActivateCase] → CaseActivated
[ScheduleInterview] → InterviewScheduled
[CompleteInterview] → InterviewCompleted
[CloseCase] → CaseClosed
```

**Phase 4: Add Actors**

```
[Case Worker] → [RegisterBeneficiary] → BeneficiaryRegistered
[Beneficiary] → [UploadDocument] → DocumentUploaded
[Verification Agent] → [VerifyDocument] → DocumentVerified
[Case Worker] → [ActivateCase] → CaseActivated
```

**Phase 5: Add Policies**

```
DocumentVerified → [Policy: Auto-activate case] → [ActivateCase] → CaseActivated
DocumentRejected → [Policy: Notify beneficiary] → [SendEmail] → EmailSent
InterviewCompleted → [Policy: Check if case ready to close] → [CloseCase] → CaseClosed
```

**Phase 6: Identify Aggregates**

**Beneficiary Aggregate**:
- BeneficiaryRegistered
- BeneficiaryStatusUpdated

**Document Aggregate**:
- DocumentUploaded
- DocumentVerified
- DocumentRejected

**Case Aggregate**:
- CaseActivated
- InterviewScheduled
- InterviewCompleted
- CaseClosed

**Phase 7: Bounded Contexts**

```
┌─────────────────────────────────┐
│ Beneficiary Management Context  │
│ - Beneficiary                   │
│ - Document                      │
│ - Case                          │
└─────────────────────────────────┘
```

---

## Common Patterns Discovered

### Pattern 1: Validation Flow
```
[SubmitData] → DataSubmitted → [Validate] → (DataValidated OR ValidationFailed)
```

### Pattern 2: Approval Workflow
```
[RequestApproval] → ApprovalRequested → [Approve/Reject] → (Approved OR Rejected)
```

### Pattern 3: Saga/Process
```
[StartProcess] → ProcessStarted → Step1Completed → Step2Completed → ProcessCompleted
                                     ↓ (if error)
                                StepFailed → [Compensate] → ProcessRolledBack
```

### Pattern 4: Time-Based
```
[Scheduler] → TimerExpired → [SendReminder] → ReminderSent
```

### Pattern 5: External Integration
```
[CallExternalAPI] → ExternalAPICallStarted → (SuccessResponseReceived OR ErrorReceived)
```

---

## From Event Storming to Architecture

### Events → NServiceBus Events
```
Orange sticky: "BeneficiaryRegistered"
→ Code: BeneficiaryRegisteredEvent.cs
```

### Commands → NServiceBus Commands
```
Blue sticky: "RegisterBeneficiary"
→ Code: RegisterBeneficiaryCommand.cs
```

### Policies → Event Handlers/Sagas
```
Purple sticky: "When document verified, activate case"
→ Code: DocumentVerifiedEventHandler.cs
```

### Aggregates → Domain Models
```
Large purple box: "Beneficiary"
→ Code: Beneficiary.cs (domain model)
```

### Bounded Contexts → Microservices/Domains
```
Context boundary: "Beneficiary Management"
→ Folder: Beneficiary/ (with Api, Domain, Endpoint.In, etc.)
```

---

## Facilitator Tips

### Starting the Session
1. **Explain the goal**: "We're discovering the business process through events"
2. **Set ground rules**: Past tense, business language, no wrong answers
3. **Start with happy path**: "What happens in the ideal scenario?"
4. **Add edge cases later**: "What if this fails?"

### Keeping Momentum
- **Time-box phases**: Don't get stuck on details
- **Park questions**: Use red sticky notes for "we'll come back to this"
- **Encourage quiet participants**: "What do you think, [name]?"
- **Refocus if wandering**: "Let's get back to the events"

### Handling Conflicts
- **Disagreements on order**: "Let's put both flows and see which makes more sense"
- **Missing information**: Mark with red sticky, research later
- **Too much detail**: "Let's zoom out - what's the high-level event?"

### Wrapping Up
1. **Photograph the board**: Document the entire timeline
2. **Identify action items**: What needs clarification?
3. **Assign ownership**: Who will create each bounded context?
4. **Schedule follow-up**: Review architecture decisions

---

## Virtual Event Storming (Miro/Mural)

### Setup
1. Create board with swim lanes for each phase
2. Add sticky note templates for each color
3. Enable real-time collaboration
4. Use voting feature for prioritization

### Best Practices
- **Keep camera on**: Engagement is key
- **Use timer**: Time-box activities
- **Breakout rooms**: For large groups, split into teams
- **Share screen**: Facilitator guides everyone

---

## Output Artifacts

After Event Storming, you should have:

1. **Event Timeline**: Complete sequence of domain events
2. **Command List**: All user actions and triggers
3. **Aggregate List**: Core domain concepts
4. **Bounded Context Map**: Domain boundaries
5. **Policy List**: Business rules and automation
6. **Questions/Issues**: Items needing research

**Next Step**: Use these artifacts to write [Requirements Document](requirements-document-template.md) and start scaffolding with [Initial Scaffolding Prompts](initial-scaffolding-prompts.md).

---

**Next**: See [Initial Scaffolding Prompts](initial-scaffolding-prompts.md) for exact GitHub Copilot prompts to generate complete domain implementations.
