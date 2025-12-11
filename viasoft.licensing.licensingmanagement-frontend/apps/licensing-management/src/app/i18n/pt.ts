
export const pt = {
  Permissions: {
    Account: 'Conta',
    AccountCreate: 'Criar Conta',
    AccountDelete: 'Remover Conta',
    AccountUpdate: 'Atualizar Conta',
    License: 'Licenciamento',
    LicenseCreate: 'Criar Licenciamento',
    LicenseDelete: 'Remover Licenciamento',
    LicenseUpdate: 'Atualizar Licenciamento',
    Software: 'Software',
    SoftwareCreate: 'Criar Software',
    SoftwareDelete: 'Remover Software',
    SoftwareUpdate: 'Atualizar Software',
    App: 'Aplicativo',
    AppCreate: 'Criar Aplicativo',
    AppDelete: 'Remover Aplicativo',
    AppUpdate: 'Atualizar Aplicativo',
    Product: 'Product',
    ProductCreate: 'Criar Produto',
    ProductDelete: 'Remover Produto',
    ProductUpdate: 'Atualizar Produto',
    BatchOperation: 'Operações em massa',
    BatchOperationInsertAppInLicenses: 'Adicionar aplicativo em licenciamentos',
    BatchOperationRemoveAppInLicenses: 'Remover aplicativo em licenciamentos',
    BatchOperationInsertProductsInLicenses: 'Adicionar produtos em licenciamentos',
    BatchOperationInsertAppsInLicenses: 'Adicionar aplicativos em licenciamentos',
    BatchOperationInsertAppsInProducts: 'Adicionar aplicativos em produtos'
  },

  common: {
    help: 'Ajuda',
    ok: 'OK',
    save: 'Salvar',
    apply_batch_operations: 'Aplicar operação em massa',
    cancel: 'Cancelar',
    add: 'Adicionar',
    remove: 'Remover',
    delete: 'Apagar',
    back: 'Voltar',
    new: 'Novo',
    save_header: 'Salvar cabeçalho',
    notification: {
      confirm_deletion: 'Você tem certeza de que deseja apagar \"{{name}}\"?',
      action_irreversible: 'Esta ação não poderá ser revertida',
      attention_batch_operation: 'Atenção o seguinte valor será aplicado para todos os itens selecionados!',
      apply_batch_operation: 'Atenção as operações em massa estão sendo aplicadas, aguarde o processo ser finalizado.',
      end_batch_operation: 'Operação em massa finalizada com sucesso.'
    },
    error: {
      invalidDate: 'Data de expiração inválida',
      expiration_date_must_be_valid: 'A data de expiração deve ser maior ou igual a data atual',
      numberOfLicensesIsRequired: 'É necessário pelo menos uma licença',
      could_not_delete: 'Não foi possível apagar \"{{name}}\"',
      could_not_create: 'Não foi possível criar \"{{name}}\"',
      could_not_edit: 'Não foi possível editar \"{{name}}\"',
      software_is_being_used: 'O software está sendo usado por outros aplicativos e/ou produtos',
      product_is_being_used: 'O produto já foi utilizado em uma licença',
      app_is_being_used: 'O aplicativo já foi utilizado em uma licença',
      account_is_being_used: 'A conta está sendo utilizada pelo licenciamento \"{{name}}\"',
      identifier_already_exists: 'O identificador já está sendo utilizado',
      field_is_required: 'Este campo é obrigatório',
      app_already_licensed: 'Um ou mais aplicativos já estão licenciados por outro(s) produto(s) e foram ignorados.',
      some_apps_already_licensed: 'Um ou mais aplicativos já estão licenciados por outro(s) produto(s) ou foram atribuidos de forma avulsa, neste caso foram ignorados.',
      number_of_licenses_equals_zero: 'O número de licenças tem que ser maior do que zero.',
      number_of_licenses_less_than_zero: 'O número de licenças não pode ser menor do que zero.',
      additional_number_of_licenses_less_than_zero: 'O número de licenças adicionais não pode ser menor do que zero.',
      cant_remove_default_app: 'O aplicativo é padrão e não pode ser removido da licença.',
      could_not_find_cnpj: 'Não foi possível encontrar um cadastro com o CNPJ/CPF informado',
      cnpj_already_registered: 'Este CNPJ já está cadastrado na conta \"{{name}}\"',
      cnpj_already_in_use: 'O CNPJ já esta sendo utilizado',
      invalid_email: 'Email inválido.',
      only_numbers: 'Por favor inserir apenas números.',
      could_not_find_adress: 'Não foi possível encontrar um endereço.',
      invalid_phone: 'Telefone inválido.',
      invalid_phone_value: 'O telefone "{{phoneNumber}}" é inválido.',
      invalid_accountId: 'Conta inválida',
      invalid_expirationDate: 'Data de expiração inválida',
      error_apply_batch_operation: 'Ocorreram erros ao aplicar a operação em massa.',
      administration_email_is_being_used: 'Este email de administrador já está sendo usado pelo licenciamento \"{{name}}\"',
      invalid_administration_email: 'Email de administrador inválido',
      AddLooseAppToLicenseErrors: {
        Unknown: 'Ocorreu um erro desconhecido ao adicionar este aplicativo avulso',
        DuplicatedIdentifier: 'Este aplicativo já está sendo utilizado nesta licença'
      },
      CouldntGetInfrastructureConfiguration: 'Não foi possível adquirir as configurações de infraestrutura'
    }
  },

  Auditing: {
    UserName: 'Nome do usuário',
    ActionName: 'Ação realizada',
    DateTime: 'Data da ocorrência',
    Details: 'Detalhes da ação',
    Title: 'Auditoria',
    BatchAction: 'Ação em massa',
    Type: 'Tipo da ação',
    Modal: {
      Close: 'Fechar',
      AuditingSeeMore: 'Detalhes de auditoria',
      Type: {
        InsertAppInLicenses: 'Adicionar aplicativo em licenciamentos',
        RemoveAppInLicenses: 'Remover aplicativo em licenciamentos',
        InsertMultipleBundlesInMultipleLicenses: 'Adicionar diversos pacotes em vários licenciamentos',
        InsertMultipleAppsInMultipleLicenses: 'Adicionar diversos aplicativos em vários licenciamentos',
        InsertMultipleAppsInMultipleBundles: 'Adicionar diversos aplicativos em vários pacotes',
        InsertMultipleAppsFromBundlesInLicenses: 'Adicionar diversos aplicativos de pacotes em licenciamentos'
      }
    }
  },

  licensings: {
    connectionInfoTitle: 'Informação sobre o consumo de licença no modo de conexão',
    accessInfoTitle: 'Informação sobre o consumo de licença no modo de acesso',
    connectionInfoOne: `Neste modo de consumo o servidor de licenças tentará buscar as licenças do aplicativo atual primeiramente em um produto,
    caso o usuário possua um produto com o aplicativo então o servidor irá realizar uma verificação para validar se o aplicativo já está em uso,
     se estiver nenhuma licença será consumida, caso contrário uma licença do produto será consumida.`,
    connectionInfoTwo: `Caso o servidor não encontre o aplicativo em um produto, então será feito uma verificação se o usuário possui o aplicativo como avulso,
     caso possua então o servidor irá realizar uma verificação para validar se o aplicativo já está em uso,
     se estiver nenhuma licença será consumida, caso contrário uma licença do aplicativo avulso será consumida.`,
    accessInfoOne: `Neste modo de consumo o servidor de licenças tentará buscar as licenças do aplicativo atual primeiramente em um produto,
   caso o usuário possua um produto com o aplicativo então o servidor irá consumir uma licença deste produto.`,
    accessInfoTwo: `Caso o servidor não encontre o aplicativo em um produto, então será feito uma verificação se o usuário possui o aplicativo como avulso,
    caso possua então o servidor irá consumir uma licença deste aplicativo avulso.`,
    infoLicenseConsumeConnection: 'A licença será consumida por conexão, não importando o número de acesso aos aplicativos.',
    infoLicenseConsumeAcess: 'A licença será consumida por acesso, consumindo licença a cada acesso aos aplicativos.',
    connectionLicenseConsume: 'Consumir licença por conexão',
    acessLicenseConsume: 'Consumir licença por acesso',
    licensing: 'Licenciamento',
    numberOfTemporaryLicenses: 'Licenças temporárias',
    licensings: 'Licenciamentos',
    accountId: 'Conta',
    status: 'Status',
    cnpjCpf: 'CNPJ/CPF licenciados',
    name: 'Nome',
    approval: 'Pendente Aprovação',
    blocked: 'Bloqueado',
    trial: 'Trial',
    active: 'Ativo',
    readOnly: 'Apenas Leitura',
    expirationDateTime: 'Data de expiração',
    numberOfDaysToExpiration: 'Número de dias até a expiração',
    numberOfLicenses: '№ Licenças',
    identifier: 'Identificador',
    additionalLicenses: 'Licenças Adicionais',
    administratorEmail: 'Email do Administrador',
    invalidEmail: 'Email inválido',
    notes: 'Observações',
    hardwareId: 'Identificador do dispositivo',
    namedLicense: 'Licença nomeada',
    offlineMode: 'Modo off-line',
    licensingModelTooltip: 'Define se o modelo da licença é nomeada ou flutuante',
    licensingModeTooltip: 'Define se o modo da licença é on-line ou off-line',
    copyTenantIdTooltip: 'Copiar Identificador',
    manageNamedUsersTitle: 'Gerenciar licenças nomeadas',
    manageNamedUsersSubTitle: {
      intro: 'Este aplicativo tem',
      middle: 'licenças disponíveis',
      mode: 'modo'
    },
    user: 'Usuário',
    addUser: 'Adicionar usuário',
    transferLicense: 'Transferir licença',
    currentUser: 'Usuário atual:',
    searchUser: 'Buscar usuário',
    errors: {
      unknownError: 'Erro desconhecido',
      notANamedLicense: 'O modelo da licença não é nomeado',
      noLicensedProductWithSuchId: 'Não foi possível encontrar o produto licenciado',
      noTenantWithSuchId: 'Não foi possível encontrar o licenciamento',
      TooManyNamedUsers: 'Você não tem mais licenças disponíveis',
      noLicensedAppWithSuchId: 'Não foi possível encontrar o aplicativo licenciado',
      namedUserEmailAlreadyInUse: 'Não foi possível adicionar a licença nomeada porque o email já está em uso'
    },
    licensesServer: {
      title: 'Servidor de Licenças',
      useHardwareIdSimpleConfigurations: 'Utilizar configurações simples de identificador de dispositivo',
      useSimpleHardwareIdInfoTitle: 'Informação sobre o uso de configurações simples de identificador de dispositivo',
      useSimpleHardwareIdInfoContent: `Essa opção deverá ser ativada para clientes que utilizam servidores em nuvem como, por exemplo, Oracle Cloud, Microsoft Azure, entre outros.
      Isso é necessário porque os provedores alteram fisicamente o lugar em que a máquina virtual é alocada, quando a mesma é reiniciada ou desligada,
      fazendo com que o identificador do dispositivo mude.`
    },
    tooltipForSagaInfo: {
      status: {
        processing: 'O status será atualizado automaticamente para ativo quando a conta do administrador for criada',
        failure: 'Houve um erro ao tentar criar a conta do administrador'
      },
      email: {
        success: 'A conta do administrador foi criada com sucesso',
        processing: 'A conta do administrador está sendo criada',
        failure: 'Houve um erro ao tentar criar a conta do administrador'
      }
    }
  },

  softwares: {
    software: 'Software',
    softwares: 'Softwares',
    new_software: 'Novo Software',
    name: 'Nome',
    identifier: 'Identificador',
    active: 'Ativo',
  },

  products: {
    main_title: 'Gerenciar produtos',
    batch_operation: 'Operação em massa',
    batch_operation_products: 'Operação em massa - produtos',
    product: 'Produto',
    products: 'Produtos',
    new_product: 'Novo produto',
    name: 'Nome',
    identifier: 'Identificador',
    active: 'Ativo',
    software: 'Software',
    custom: 'Personalizado',
    addAppInLincense: 'Adicionar aplicativo nos licenciamentos existentes para este produto?',
    removeAppInLicense: 'Remover o aplicativo nos licenciamentos existentes para este produto?'
  },
  apps: {
    main_title: 'Gerenciar aplicativos',
    batch_operation: 'Operação em massa',
    batch_operation_products: 'Operação em massa - produtos',
    batch_operation_licenses: 'Operação em massa - licenciamentos',
    app: 'Aplicativo',
    apps: 'Aplicativos',
    apps_in_product: 'Aplicativos no produto ',
    new_apps: 'Novo aplicativo',
    active: 'Ativo',
    name: 'Nome',
    identifier: 'Identificador',
    software: 'Software',
    loose: 'Aplicativos avulsos',
    default: 'Padrão',
    domains: {
      domain: 'Domínio',
      VD: 'Vendas',
      CP: 'Compras',
      FA: 'Faturamento',
      FI: 'Financeiro',
      RMA: 'RMA',
      LG: 'Logística',
      FC: 'Fiscal',
      CT: 'Contábil',
      EN: 'Engenharia',
      PD: 'Produção',
      MT: 'Manutenção',
      QA: 'Qualidade',
      RH: 'Recursos humanos',
      CG: 'Configurações',
      DEV: 'Desenvolvimento',
      CST: 'Customizados',
      LS: 'Licenciamento',
      MOB: 'Mobile',
      REP: 'Relatórios',
      PROJ: 'Projetos'
    }
  },
  accounts: {
    accounts: 'Contas',
    account: 'Conta',
    generalInfo: 'Informações Gerais',
    adressInfo: 'Endereço da Conta',
    billingInfo: 'Endereço de Cobrança',
    sync: 'Sincronizar com CRM',
    details: {
      companyName: 'Nome/Razão Social',
      cnpjCpf: 'CNPJ/CPF',
      phone: 'Telefone',
      webSite: 'Site web',
      email: 'Email',
      billingEmail: 'Email de cobrança',
      tradingName: 'Conta/Fantasia',
      accountStatus: {
        status: 'Status',
        active: 'Ativa',
        blocked: 'Inativa'
      },
      street: 'Rua',
      number: 'Número',
      detail: 'Complemento',
      neighborhood: 'Bairro',
      city: 'Cidade',
      state: 'Estado',
      country: 'País',
      zipCode: 'CEP'
    },
    notification: {
      started_sync_title: 'Sincronização em progresso',
      started_sync: 'Você receberá uma notificação quando a sincronização terminar.'
    }
  },
  filequota: {
    title: 'Cotas de Armazenamento',
    selectedApp: 'App selecionado',
    addApp: {
      title: 'Selecione um app para adicionar sua cota',
      search: 'Pesquise por app',
      grid: {
        appName: 'Nome do App'
      }
    }
  },
  file_provider: {
    configurationTitle: 'Configurações de Armazenamento de Arquivos',
    grid: {
      appName: 'Aplicativo',
      quotaLimit: 'Limite atual'
    },
    form: {
      maxSizePerFile: 'Tamanho permitido por arquivo (MB)',
      quotaLimit: 'Tamanho de Cota permitida (MB)',
      tenantQuotaLimit: 'Tamanho da cota do Tenant (MB)',
      byteSizeNoLimitHint: '-1 para deixar ilimitado',
      acceptedExtensions: 'Extensões permitidas',
      acceptedExtensionsHint: 'Lista de extensões (com .) separados por vírgula (deixe em branco para aceitar todos)',
      errors: {
        limitMustBeSet: 'Um limite deve ser devido, deixe -1 para deixar ilimitado',
        unknownErrorDuringSave: 'Erro ao tentar salvar, tente novamente mais tarde'
      }
    }
  },
  infrastructureConfiguration: {
    title: 'Configurações de infraestrutura',
    gateway: 'Endereço de entrada',
    desktopDatabaseName: 'Nome do banco de dados do sistema Desktop',
    error: {
      invalidGateway: 'Gateway inválido'
    }
  },
  Organization: {
    Title: 'Organização',
    Unit: {
      Title: 'Unidade Organizacional',
      Activate: 'Ativar',
      Deactivate: 'Desativar',
      AddEnvironment: 'Adicionar Ambiente',
      EditUnit: 'Editar',
      SelectAnUnitToViewEnvironments: 'Selecione uma unidade organizacional para visualizar seus ambientes',
      Fields: {
        Name: 'Nome',
        Description: 'Descrição'
      },
      Errors: {
        NameConflict: 'Nome com conflito, renomeie para continuar',
        Unknown: 'Erro desconhecido, tente novamente mais tarde',
      },
      Warnings: {
        EnvironmentsWillNotBeActivatedAutomatically: 'Os ambientes pertencentes a esta unidade não serão ativos automaticamente.'
      }
    },
    UnitEnvironment: {
      Title: 'Ambiente',
      Environments: 'Ambientes',
      EnvironmentOf: 'Ambientes pertencentes a unidade \"{{unitName}}\"',
      Activate: 'Ativar',
      Deactivate: 'Desativar',
      CopyEnvironmentIdTooltip: 'Copiar Id do ambiente',
      Fields: {
        Name: 'Nome',
        Description: 'Descrição',
        IsProduction: 'É Produção',
        IsMobile: 'É Mobile',
        IsWeb: 'É Web',
        IsDesktop: 'É Desktop',
        DatabaseName: 'Nome do Banco',
        Version: 'Versão do Banco Desktop'
      },
      Errors: {
        InvalidDbVersion: 'A versão do banco de dados deve estar no formato correto. Exemplo: 2023.1.2 ou 2023.12.34',
        InvalidDbNameRegexValidate: 'O nome do banco de dados não pode conter caracteres especiais tais como acentos, simbolos e espaços.',
        NameConflict: 'Nome com conflito, renomeie para continuar',
        OrganizationUnitNotFound: 'Erro desconhecido, tente novamente mais tarde',
        InProductionConflict: 'Já existe um ambiente destinado para Produção, desmarque para continuar',
        NotTypedEnvironment: 'Marque o ambiente como Desktop ou Web para continuar',
        InvalidDatabaseName: 'Nome do banco inválido, preencha para continuar',
        Unknown: 'Erro desconhecido, tente novamente mais tarde',
        EmptyDatabase: 'O nome do banco é obrigatório quando o campo desktop está marcado'
      }
    }
  }
};
