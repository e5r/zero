Zero
====

**Zero** é um serviço de configurações centralizado para softwares. Recebe este nome
porque pretende ser o primeiro passo no desenvolvimento de qualquer software, ou seja,
antes do passo 1 do desenvolvimento, temos o **passo zero** que é iniciar a configuração
do ambiente de desenvolvimento do mesmo.

## Filosofia

Construa softwares em qualquer tecnologia que *não precisam* de configuração inicial,
ou seja, **zero config**, mas que permitem cofigurações customizadas de forma segura.

Isso significa que seu software pode ser iniciado em um modo padrão, e caso hajam
outras configurações, ele pode se adaptar.

## Problema que se pretende resolver

Quando se está aprendendo a programar, o programa inicial é o famoso *alô mundo*.
Este é o projeto mais básico que se pode escrever, mas em nada tem haver com um
software do mundo real.

No mundo real temos acesso a bancos de dados, outros serviços conectados, e com isso
precisamos de dados de acesso, senhas, e muitas outras preferências.

Claro que hoje temos [docker](https://www.docker.com) e [kubernetes](https://kubernetes.io),
mas ainda assim precisaremos de um certo cuidado com a proteção de nossos dados sensíveis.

Agora, imagine que você tem um local central de configurações disponível na rede,
e que é garantido um tráfego seguro dos dados entre as máquinas, e que você pode
criar a qualquer momento um container para sua nova aplicação, e depois que ela esteja
pronta, você pede simplesmente a um gestor do ambiente de produção que configure-a
de acordo com as necessidades de produção, e sem se preocupar em te passar nenhum
dado sensível. **É isso que pretendemos fazer**, e isso resolveria muitos problemas
de configuração de aplicativos desde seu desenvolvimento até implantação em produção.

## Como funciona?

Temos 3 componentes envolvidos:

1. Um servidor de configurações
2. Um utilitário de configurações
3. Uma biblioteca de acesso a configurações

### 1. Servidor de configurações

Um servidor [gRPC](https://grpc.io) pronto a atender basicamente 8 requisições:

1. Criar novo par de chave assimétrica de acesso
2. Listar chaves de acesso
3. Transferir chaves de acesso
4. Revogar uma chave de acesso
5. Criar um novo container de configuração
6. Listar os container's de configuração
7. Transferir valores de configurações em um container de configuração
8. Definir valores de configurações em um container de configuração

### 2. Utilitário de configurações

Um utilitário de linha de comando que permite requisitar as funções do servidor
de configurações.

* Em toda requisição uma chave deve ser informada
* Todo dado antes de ser enviado ao servidor é criptografado com a chave
  privada informada. E toda resposta do servidor é também criptografada com a
  chave pública informada. Assim é garantida a criptografia de ponta a ponta.
* Durante toda requisição a impressão digital da chave é transportada

### 3. Biblioteca de acesso a configurações

Uma biblioteca em qualquer linguagem suportada, que dada uma chave de acesso
é capaz de obter somente, qualquer configuração de um container de configuração
pré-definido.

## Sobre as configurações

1. O que é um container de configuração?

É um identificador único no servidor que atenda aos critérios abaixo:
* Não contenha espaços
* Não contenha caracteres especiais além de `/.-_+`
* Contenha pelo menos 3 caracteres alfanuméricos
* Inicie com um caractere alfabético ou `_` (sublinhado)

2. O que é uma entrada de configuração?

É um identificador dentro de um container de configuração com um nome
que atenda aos critérios abaixo:
* Não contenha espaços
* Somente caracteres alphanuméricos
* Inicie com um caractere alfabético ou `_` (sublinhado)

3. O que é um valor de entrada de configuração?

Cada entrada de configuração pode ter um dos seguintes valores:

* `nil` - Nenhum valor informado
* `byte` - Valor de 1 byte (8-bits)
* `int32` - Valor de 4 bytes (32-bit signed integer, two's complement)
* `int64`	- Valor de 8 bytes (64-bit signed integer, two's complement)
* `uint64` - Valor de 8 bytes (64-bit unsigned integer)
* `double` - Valor de 8 bytes (64-bit IEEE 754-2008 binary floating point)
* `decimal128` - Valor de 16 bytes (128-bit IEEE 754-2008 decimal floating point)

> Dados baseados na especificação [BSON](https://bsonspec.org/spec.html)

Também tem uma lista de entradas filhas.

4. O que é um namespace de entrada de configuração?

Quando o cliente solicita um valor de configuração, ele na verdade informa
um namespace que contém a localização de uma entrada qualificada, e a isso
damos o nome de namespace.

Imagine que você tenha uma entrada chamada `database` e ela tenha o valor
`nil`, e uma lista de entradas filhas com: `host`, `port`, `user`, `pwd`.

Se você solicitar o valor completo de configuração `database` receberá algo
como isto:
```json
{
  "value": null,
  "childs": {
    "host": { "value": "hostname", "childs": [] },
    "port": { "value": 123, "childs": [] },
    "user": { "value": "username", "childs": [] },
    "pwd": { "value": "password", "childs": [] },
  }
}
```

Você também poderia solicitar somente o valor explícito para `database`,
o que retornaria:
```json
null
```

Mas se preferisse poderia obter o valor explícito diretamente de um
filho como `database.host`, que retornaria:
```json
"hostname"
```

Agora imagine a combinação de vários níveis de objetos filhos.
Com isso você pode acessar um valor de configuração como se fosse
um espaço de nomes acessíveis hierarquicamente.

## Algumas notas

* Ao instalar o servidor é definida uma chave de acesso mestra, e só essa
  chave é capaz de criar outras chaves, além de gerenciar todas as demais.
* Toda a comunicação é criptografada de ponta a ponta usando as chaves de
  acesso.
* A chave mestra só executa as requisições de 1 a 4, e não pode executar
  as ações de 5 a 8
* Uma chave não mestra só executa as requisições de 5 a 8, e não pode
  executar as ações de 1 a 4
* Uma chave de acesso compartilhada de desenvolvimento é criada e entregue
  a todos os desenvolvedores do time para todos ou para projetos específicos.
* Durante a implantação em outros ambientes, uma chave de acesso para cada
  aplicativo é criada e essa fica em poder apenas do administrador da
  infraestrutura.

