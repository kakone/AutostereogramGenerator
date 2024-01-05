# AutostereogramGenerator
A simple autostereogram generator

## Usage
```C#
var autostereogramGenerator = new AutoStereogramGenerator(depthMap, optionalPattern);
var autostereogram = autostereogramGenerator.Create();
autostereogram.SaveAsPng(saveFileName);
```

[![NuGet Status](http://img.shields.io/nuget/v/Autostereogram.svg?style=flat)](https://www.nuget.org/packages/Autostereogram)

## Have fun
You can also download and have fun with the [AutostereogramGenerator application](https://github.com/kakone/AutostereogramGenerator/releases) :

![Autostereogram application](AutostereogramGenerator.png)