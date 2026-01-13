# Markdown reference guide

THE Project uses GitHub flavored markdown. It also supports [Docfx flavored markdown controls](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html?tabs=tabid-1%2Ctabid-a). This reference guide documents provides a examples of the markdown syntax you can use.

## Headings

```markdown
# H1
## H2
### H3
```

## Bold

```markdown
**bold text**
```

## Italic

```markdown
*italicized text*
```

## Blockquote

```markdown
> blockquote
```

## Ordered List

```markdown
1. First item
2. Second item
3. Third item
```

## Unordered List

```markdown
- First item
- Second item
- Third item
```

## Inline code

```markdown
Run the command `az group create --location westeurope --name myResourceGroup`
```

## Horizontal Rule

```markdown
---
```

## Link

```markdown
[alt text](https://www.google.com)
```

## Image

```markdown
![CDF logo](./media/markdown-reference/cdf-logo.png)
```

## Table

```markdown
| Syntax    | Description |
|-----------|-------------|
| Header    | Title       |
| Paragraph | Text        |
```

## Fenced Code Block

```markdown
    ```json
    {
      "firstName": "John",
      "lastName": "Smith",
      "age": 25
    }
    ```
```

## Strikethrough

```markdown
~~The world is flat.~~
```

## Alert

```markdown
> [!NOTE]
> Add note you want to have stand out to users.
```

```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```

:::mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
:::
