Update Version
==========
* update `<Version>` value in `Coroutine.NET.csproj`

Release to Nuget
==========
1. Create a new branch called `release/X.Y.Z`
2. `git checkout -b release/X.Y.Z`
3. Change the version in `Coroutine.NET.csproj` to your desired release version (see `Update Version`)
4. `git commit -am "Create release X.Y.Z."` (where X.Y.Z is the new version)
5. `git tag -a X.Y.Z -m "Version X.Y.Z"` (where X.Y.Z is the new version)
6. The CI pipeline will recognize the tag and upload the artifacts to nuget and generate changelog automatically
7. Push to github with `git push && git push --tags`
8. Create a PR to merge to main
