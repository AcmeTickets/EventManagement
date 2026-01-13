# Branching Strategy

A branching strategy is basically set of rules which describes naming guidelines for branches, when branches should be created, what use branches should have. Team members can publish, share, review, and iterate on code changes through branches shared with others.

## Use Release Flow

You should follow [Release Flow](https://docs.microsoft.com/devops/develop/how-microsoft-develops-devops) when working within the Pace Docs repository. Whenever you introduce a set of logically related changes, it's a best practice to create a working branch to manage your changes through the workflow. We refer to it here as a working branch because it's a workspace to iterate/refine changes, until they can be integrated back into the master branch.

Isolating related changes to a specific branch allows you to control and introduce those changes independently, targeting them to a specific release time in the publishing cycle. In reality, depending on the type of work you do, you can easily end up with several working branches in your repository. It's not uncommon to be working on multiple branches at the same time, each representing a different contribution.

## Branch naming convention

When naming your branch, use the Release Flow branch naming convention **users/[YOUR_NAME]/[BRANCH_NAME]** where **YOUR_NAME** is your name and **BRANCH_NAME** is a one or two word description of your change.

Branch names should follow kebab naming conventions. They should contain no spaces, special characters, or capital letters. Use dashes **-** for spaces.

## How to create a branch

Create a new branch in your local repository for capturing your proposed changes by running the git checkout command with an appropriately named branch parameter. For example, if John Smith is creating a branch with Network Security Group design related content, he would create his branch with the commands below:

### Branch off current branch

```cmd
git checkout -b users/smith/nsg-design-content
```

Note that the command above will create a new branch from your current branch which might be the master branch or another branch you might have previously created. Whenever you create a new branch it is required that your newly created branch is up to date with the master branch to avoid conflicts during the merge process.

### Branch off master

You can create a new branch and have your newly created branch pull from the master branch to make your newly created branch be up to date with the master branch with the command below:

```cmd
git checkout -b users/smith/nsg-design-content master
```

By adding the name of the branch master that you want your newly branch to be up to date with at the end of the cmd, your newly created branch pulls from the master branch and is up to date with the master branch.

## Branch status

To check your current working branch and it's status, use the command below

```cmd
git status
```

To view a list of the branches in your local workspace, use the command below

```cmd
git branch
```
