Zero
====

**Zero** é um serviço de configurações centralizado para softwares. Recebe este nome
porque pretende ser o primeiro passo no desenvolvimento de qualquer software, ou seja,
antes do passo 1 do desenvolvimento, temos o **passo zero** que é iniciar a configuração
do ambiente de desenvolvimento em si.

## Continuous status

Branch | Status
:----- | :-----
`build` **master** | ![](https://github.com/e5r/zero/actions/workflows/ci.yml/badge.svg?branch=master)
`build` **develop** | ![](https://github.com/e5r/zero/actions/workflows/ci.yml/badge.svg?branch=develop)
`coverage` **master** | ![](https://codecov.io/gh/e5r/zero/branch/master/graph/badge.svg?token=LMUB5UDA11)
`coverage` **develop** | ![](https://codecov.io/gh/e5r/zero/branch/develop/graph/badge.svg?token=LMUB5UDA11)

## Pacotes NuGet

Package  | Status
:------ | :-----
E5R.Zero.Net.Client | [![](https://img.shields.io/nuget/v/E5R.Zero.Net.Client.png?style=flat-square)](https://www.nuget.org/packages/E5R.Zero.Net.Client)

## Começando com a versão em desenvolvimento

1. Adicione o Feed E5R em suas origens NuGet
```sh
$ dotnet nuget add source --username USERNAME \
  --password PASSWORD --store-password-in-clear-text \
  --name github-e5r "https://nuget.pkg.github.com/e5r/index.json"
```

2. Adicione o pacote em seu projeto:
```sh
dotnet add PROJECT package E5R.Zero.Net.Client --version *
```
