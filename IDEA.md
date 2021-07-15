Filosofia por tráz do E5R Zero
==============================

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
criar a qualquer momento um local para guardar as configurações de sua nova aplicação,
e depois que ela (sua aplicação) esteja pronta, você pede simplesmente a um gestor do
ambiente de produção que configure-a de acordo com as necessidades de produção, e que ele
não precisa se preocupar em te passar nenhum dado sensível.

**É isso que pretendemos fazer!** Talvez isso resolva muitos problemas de configuração
de aplicativos desde seu desenvolvimento até implantação em produção. E a única coisa
que sua aplicação precisará saber é onde está o servidor de configurações.

## Como funciona?

Temos 3 componentes envolvidos:

1. Um servidor de configurações
2. Um utilitário de configurações
3. Uma biblioteca de acesso a configurações

### 1. Servidor de configurações

Um servidor [gRPC](https://grpc.io) pronto a atender basicamente 7 requisições:

1. Registrar novas chaves de acesso
2. Listar chaves de acesso
3. Revogar chaves de acesso
4. Criar um novo container de configurações
5. Listar os container's de configurações
6. Transferir valores de configurações
7. Definir valores de configurações

### 2. Utilitário de configurações

Um utilitário de linha de comando que permite requisitar as funções do servidor
de configurações.

* Em toda requisição a impressão digital da chave deve ser informada
* Todo dado antes de ser enviado ao servidor é criptografado com a chave
  privada informada. E toda resposta do servidor é também criptografada com a
  chave pública informada. Assim é garantida a criptografia de ponta a ponta

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

Cada entrada de configuração pode estar vazia (`null`) ou conter uma `string`.

> Toda `string` é UTF-8

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
    "host": { "value": "hostname", "childs": {} },
    "port": { "value": 123, "childs": {} },
    "user": { "value": "username", "childs": {} },
    "pwd": { "value": "password", "childs": {} },
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

### Uma coisa legal

Com essa estrutura onde podemos ter em uma entrada tanto um valor
quanto uma lista de filhos nos permite criar uma configuração interessante.

Imagine que você tenha que configurar o acesso ao banco de dados de uma
aplicação. Você poderia fazer isso facilmente com uma primeira entrada chamada
`database`, e os filhos seriam as propriedades de acesso ao banco como:
`host`, `port`, `user` e `password`. Logo, sua configuração seria mais ou menos assim:
```json
{
  "value": null,
  "childs": {
    "host": { "value": "hostname", "childs": {} },
    "port": { "value": 123, "childs": {} },
    "user": { "value": "username", "childs": {} },
    "pwd": { "value": "password", "childs": {} },
  }
}
```

Mas o fato é que sua aplicação pode se conectar em vários tipos de banco de
dados, como: `postgres`, `sqlite` ou `mssql`. Como fazer?

Simples. Adicionamos uma propriedade chamada `type` para informar o tipo do banco.
Então ficamos assim:
```json
{
  "value": null,
  "childs": {
    "type": { "value": "postgres", "childs": {} },
    "host": { "value": "hostname", "childs": {} },
    "port": { "value": 123, "childs": {} },
    "user": { "value": "username", "childs": {} },
    "pwd": { "value": "password", "childs": {} },
  }
}
```

Agora basta obter a configuração `database.type`, com isso saberemos o tipo de
banco e então basta obter o restante das informações com as demais chaves:
`database.host`, `database.port`, `database.user` e `database.pwd`.

Então sempre que mudar o tipo de banco, precisamos apenas alterar os valores
para tal.

Só que considere que para cada tipo de banco as propriedades filhas serão
diferentes. Algo como: para o tipo `sqlite` só precisamos da propriedade `file`,
e para o tipo `mssql` da propriedade `connection-string` e já para o tipo `postgres`
precisamos de `host`, `port`, `user` e `pwd`.

É bem fácil fazer as mudanças, mas imagine que na verdade o que você pretende
é que hajam as configurações definidas para os 3 tipos de banco, e a aplicação
é que vai escolher qual ela quer usar. Então você precisaria deixar já prontas
todas as opções.

Podemos fazer simples assim:
```json
{
  "value": null,
  "childs": {
    "type": { "value": "postgres", "childs": {} },
    "sqlite": {
      "value": "file://myfile.sqlite",
      "childs": {}
    },
    "mssql": {
      "value": "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;",
      "childs": {}
    },
    "postgres": {
      "value": null,
      "childs": {
        "host": { "value": "hostname", "childs": {} },
        "port": { "value": 123, "childs": {} },
        "user": { "value": "username", "childs": {} },
        "pwd": { "value": "password", "childs": {} },
      }
    }
}
```

Agora basta continuar obtendo a configuração `database.type`, e caso seja do tipo
`postgres` basta obter os demais valores: `database.postgres.host`,
`database.postgres.port`, `database.postgres.user` e `database.postgres.pwd`.

Se for do tipo `sqlite` obtenha o valor do arquivo de banco com `database.sqlite`,
e se for do tipo `mssql` obtenha o valor da *connection string* com `database.mssql`.

Isso resolve nosso problema, mas poderia ficar um pouco melhor.

Vejamos algumas mudanças que podemos fazer:
```json
{
  "value": "postgres",
  "childs": {
    "sqlite": {
      "value": "file://myfile.sqlite",
      "childs": {}
    },
    "mssql": {
      "value": "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;",
      "childs": {}
    },
    "postgres": {
      "value": null,
      "childs": {
        "host": { "value": "hostname", "childs": {} },
        "port": { "value": 123, "childs": {} },
        "user": { "value": "username", "childs": {} },
        "pwd": { "value": "password", "childs": {} },
      }
    }
}
```

Observe o que mudamos:

* Removemos a entrada `type` e colocamos o valor diretamente em "value" na raiz

Na verdade nós só simplificamos um pouco as coisas, e agora ao invés de buscar
por `database.type` para saber o tipo do banco, basta  buscar por `database`, e o
resto segue como antes.

Mas agora, se você já desenvolve algo que conecte a um banco de dados, deve saber
que você pode guardar as configurações de duas formas.

1. Guardando diretamente uma *connection string* como:
**Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;**

2. Ou também poderia guardar as informações separadamente como:
* Server=myServerAddress
* Database=myDataBase
* Trusted_Connection=True

E depois jutar isso em uma conection string.

Então seria bom guardar os valores nas duas formas, e deixar a própria aplicação
escolher o que melhor convém pra ela.

Então vamos mudar a configuração para:
```json
{
  "value": "mssql",
  "childs": {
    "sqlite": {
      "value": "file://myfile.sqlite",
      "childs": {
        "fileName": { "value": "myfile.sqlite", "childs": {} }
      }
    },
    "mssql": {
      "value": "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;",
      "childs": {
        "server": { "value": "myServerAddress", "childs": {} },
        "database": { "value": "myDataBase", "childs": {} },
        "trusted_connection": { "value": true, "childs": {} }
      }
    },
    "postgres": {
      "value": "Server=127.0.0.1;Port=5432;Database=myDataBase;User Id=myUsername;Password=myPassword;",
      "childs": {
        "host": { "value": "127.0.0.1", "childs": {} },
        "port": { "value": 1254323, "childs": {} },
        "db": { "value": "myDataBase", "childs": {} },
        "user": { "value": "myUsername", "childs": {} },
        "pwd": { "value": "myPassword", "childs": {} },
      }
    }
}
```

Agora, apesar de termos aumentado nossa configuração, demos a aplicação a flexibilidade
no obter as configurações.

Agora a aplicação pode buscar o tipo da configuração com `database`, e com base no tipo ela
pode obter ou a *connection string* completa com `database.mssql`, ou pode obter cada
pedaço da configuração com `database.mssql.server`, `database.mssql.database` e
`database.mssql.trusted_connection`. O que importa é que fica a critério da aplicação
escolher o melhor método para ela usar, você como administrador de infraestrutura, deu
a faca e o queijo pra ela.


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

