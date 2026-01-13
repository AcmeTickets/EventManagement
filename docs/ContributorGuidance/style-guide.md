# Style guide

This document lists guidelines to help you write technical content that is direct,  clear, and concise.  You must follow these guidelines to author an informative technical document:

## Make your content scannable

Readers want content that is relevant and easy to digest. You must help the reader locate what they need to consume and what they can ignore. Split the document up into as many sub-sections as it makes sense.

You can use the following constructs to organize text into discrete components to support scanning:

### Headings

Headings provide structure and visual points of reference to help readers scan content.

Consider the following guidelines while using headings:

- Keep a specific and shortest possible heading.
- Use suitable heading level. If a document is long, introduce different heading levels.
- Readers should be able to decide whether to read content or skip it based on the heading itself.

### Lists

Lists increase readability by generating white space and highlights key facts, stats, or data.

Consider the following guidelines while using a list:

- Provide an introductory text to make sure the list has a clear purpose.
- Use one main idea per section.
- Use a number list if the items are sequential or listed by their priority.
- Use the bulleted list of items with the same level of importance level but do not have any particular order.
- It’s OK to have a couple of short sentences in a list item.

### Quotes

A quote is an excerpt from someone important to the story. In technical writing, it can provide a note or an interesting fact related to the content. It adds visual interest to text-heavy content. E.g.

> More than 1 billion authentications take place in Azure Active Directory every day.

### Tables

Tables are powerful tools to convey complex sets of data. It makes complex information easier to understand by presenting it in a clear structure. Use tables to distinguish a set of related items across different parameters.

- Make sure the purpose of the table is clear. If necessary, include a table title or brief introduction.
- Don’t leave a cell blank to indicate there’s no entry for that cell. Instead, use *Not applicable* or *None.*

For example, the following table differentiates a number of VMs across memory, cores, disks, and pricing in a very readable manner.

| VM name          | # Cores | Memory (GiB) | Max # disks | Linux price |
| :--------------- | ------: | -----------: | ----------: | ----------: |
| Standard_DC2s_v2 |       2 |            8 |           2 |       0.514 |
| Standard_DC1s_v2 |       1 |            4 |           1 |       0.257 |
| Standard_DC8_v2  |       8 |           32 |           8 |       2.056 |
| Standard_DS14    |      16 |          112 |          64 |       1.542 |

### Images

A compelling image or infographic can capture a reader's attention much more quickly than a sentence. Images provide periodic breaks between blocks of text. The images used in the document must elaborate the concept.

## Use consistent Language

You must use formal English written in direct and simple terms. Avoid saying the same thing twice and repeating the same word in a sentence. Consistent use of language, grammar, spelling, abbreviation, capitalization, highlighting, and  formatting set standards of your documents.

### Active and passive voice

Use active voice whenever possible. It distinctly identifies the subject and the action taken by the subject. Use passive voice only when it is appropriate for emphasis or when you lack clarity.

### Keep it short

Every word must have a place in the sentence and a meaning. Instead of putting commas in a sentence, consider splitting it into two or more sentences for improved clarity.

A paragraph should not be longer than four to five sentences. You must treat a paragraph as a place to introduce a single idea or a concept. There should be a new paragraph to expand the concept. All of the ideas contained within a paragraph must relate to one central thought

### Use bias-free language

You must consider the following:

- Avoid awkward he or she phrasing. Use gender neutral noun/pronoun instead or use plural.
- Don't use terms that may carry unconscious racial bias or terms associated with military actions, politics, or historical events and eras.
- Don't make generalizations about people, countries, regions, and cultures.

Please refer to [this](https://docs.microsoft.com/style-guide/bias-free-communication) guide for more details.

### Use consistent grammar

Technical writing should convey coherent ideas. It is not possible without using a consistent grammar.

These are the few basics rules that you must follow:

- **Capitalization**: You must use sentence-style capitalization. That means everything is lowercase except the first word and proper nouns (including names of brands, products, and services). You must follow the same in headings as well.
- **Verbs**: Use present tense whenever you can.
- **Contractions**: Use common contractions, such as *it’s, you’re, that's,* and *don’t*. It creates a friendly and informal tone. Be consistent with it. For example don’t use *can’t* and *cannot* in the same content.
- **Prepositions**: Avoid joining more than two phrases with prepositions. Please refer to [this]([Prepositions - Microsoft Style Guide | Microsoft Docs](https://docs.microsoft.com/style-guide/grammar/prepositions)) guide for more details.
- **Punctuations**: Punctuations provide clues for reader to pause. The more punctuations you add, the more complex a sentence becomes. Please refer to [this](https://docs.microsoft.com/style-guide/punctuation/) guide for more details.

## Follow basic markdown syntax

Formatting makes a document readable, understandable and keeps the reader interested in the content. You must be consistent with the formatting and styling of the technical document. The document should be easy to read and navigate across. Follow the basic markdown formatting techniques. Refer to [this](markdown-reference.md) document for the basic markdown syntax.

## Use authoring tools

You can use simple text editors to author your document.  However, we recommend authors to install the [Docs Authoring Pack](https://docs.microsoft.com/contribute/how-to-write-docs-auth-pack). It is a list of Visual Studio Code add-ons that will help you author your markdown documents.

At the least, you must have a markdown editor with preview options. These are some of the popular markdown editors available.

- Typora
- GhostWriter
- Remarkable
- UberWriter

### Follow best practices

You must preserve the standard of the documentation by following these best practices:

#### Title and Introduction

All pages must include the title and an introduction. An Introduction should be one or two paragraphs which explain the purpose of the documentation with no assumption of prior knowledge and understanding of the technology.

#### Define Terminology and acronyms

Each new technical term must be highlighted using *term* Markdown syntax for its first use. The meaning of the term should be placed after the term itself in brackets. Similarly, each acronym should be first introduced highlighted in its expanded form followed by it in brackets (For example"*Entity Framework (EF)*"). This is true even for terms the reader should be expected to know.

#### Split documents for clarity

You must split a document into two or more new documents if it provides more clarity.

#### Avoid duplication

Avoid duplication of any content available in [doc.microsoft.com](https://doc.microsoft.com). Use links to this external content  whenever possible.
