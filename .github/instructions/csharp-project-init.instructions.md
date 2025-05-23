請遵循以下流程檢查並設定開發環境。

please do git commit for each step when you run command of edit files. If nothing to commit, just ignore it.

1. Make sure git command line tool has been installed.

    ```ps1
    git --version
    ```

   If there is no Git CLI been installed, please stop execution.

2. Make sure the working tree is clean.

    ```ps1
    git status
    ```

   If the working directory is not clean, please stop execution.

3. Check for .NET SDK version. It must be larger than `9.0.100`.

    ```ps1
    dotnet --version
    ```

4. Use `dotnet` CLI to create project by using `webapi` template without assign `-n` argument.

    ```ps1
    dotnet new webapi -controllers
    ```

5. Add Entity Framework Core 9 and related SQL Server NuGet packages. Don't use prerelease version.

6. Check for EF Core Power Tools CLI has been installed.

    ```ps1
    efcpt --version
    ```

   If efcpt is not installed yet, or if the version is lower than `9.1.807`, please reinstall to the latest version:

    ```ps1
    dotnet tool update ErikEJ.EFCorePowerTools.Cli -g --version 9.*
    ```

7. Setup C# Global Usings in the project.

   The filename must be `GlobalUsings.cs`. Add some common namespace to it.

8. Add `.gitignore` file

    ```ps1
    dotnet new gitignore
    ```

9. Add `.gitattributes` file that avoid cross-platform issues.

   Here is the default template for `.gitattributes`. Add the content to the `.gitattributes` file.

    ```txt
    # Set default behavior to automatically normalize line endings.
    * text=auto

    # Force batch scripts to always use CRLF line endings so that if a repo is accessed
    # in Windows via a file share from Linux, the scripts will work.
    *.{cmd,[cC][mM][dD]} text eol=crlf
    *.{bat,[bB][aA][tT]} text eol=crlf

    # Force bash scripts to always use LF line endings so that if a repo is accessed
    # in Unix via a file share from Windows, the scripts will work.
    *.sh text eol=lf
    
    .env text eol=lf
    Dockerfile text eol=lf

    # Denote all files that are truly binary and should not be modified.
    *.mp3 binary
    *.wav binary
    *.bmp binary
    *.png binary
    *.jpg binary
    *.gif binary
    ```

   > Reference: [.gitattributes Best Practices](https://rehansaeed.com/gitattributes-best-practices/)

10. Download editorconfig from this url

curl -sL https://gist.github.com/jim60105/ae6ba63978a2dc3ffb3ebb77344cc7f7/raw/47f342c4b793a32697af6d62022692c26f849c07/.editorconfig > .editorconfig

Let's do this step by step.
