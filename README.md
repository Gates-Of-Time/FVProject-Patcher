## FVP Patcher
[![CD Build](https://github.com/Gates-Of-Time/FVProject-Patcher/actions/workflows/cd.yml/badge.svg)](https://github.com/Gates-Of-Time/FVProject-Patcher/actions/workflows/cd.yml)
[![CI Build](https://github.com/Gates-Of-Time/FVProject-Patcher/actions/workflows/ci.yml/badge.svg)](https://github.com/Gates-Of-Time/FVProject-Patcher/actions/workflows/ci.yml)

## Requirements
Patcher requires [.NET Desktop Runtime 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to run.

`Wine` users can try [this](https://www.winehq.org/pipermail/wine-devel/2020-August/172472.html) approach to install the .Net runtime/sdk.

Latest release can be found under [Releases](https://github.com/Gates-Of-Time/FVProject-Patcher/releases).

## Server admins
To create the patch server side, please use [Xackerys' patch creator](https://github.com/xackery/eqemupatcher).

PS! Currently the source is hardcoded to FV Projects host url's.

## Build release
Push a new tag with the new version number for the release on the main branch
```
git tag {1.0}
git push --tags
```

* Go to [Releases](https://github.com/Gates-Of-Time/FVProject-Patcher/releases)
* Create a new release based on the new tag.
  This will trigger a new build creating publish artefacts and attach them to the release.

