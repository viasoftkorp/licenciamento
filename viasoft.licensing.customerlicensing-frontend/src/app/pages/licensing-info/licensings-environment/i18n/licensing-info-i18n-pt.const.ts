export const LICENSING_ENVIRONMENT_I18N_PT= {
  Environment: {
    Organization: {
      AddOrganizationUnit: 'Adicionar unidade organizacional',
      Units: 'Unidades organizacionais',
      UnitsFieldName: {
        Name: 'Nome',
        Description: 'Descrição'
      },
      Tooltips: {
        Add: 'Adicionar ambiente',
        Edit: 'Editar unidade',
        Activate: 'Ativar unidade',
        Deactivate: 'Desativar unidade'
      },
      AddUnitModal: {
        Title: 'Unidade organizacional',
        Name: 'Nome',
        Description: 'Descrição',
        Save: 'Salvar',
        Cancel: 'Cancelar',
        Errors: {
          unknown: 'Erro desconhecido, tente novamente mais tarde',
          NameConflict: 'Nome com conflito, renomeie para continuar',
          OrganizationNotFound: 'Organização não encontrada',
          IdConflict: 'Conflito de Id'
        }
      },
      AddEnvironmentModal: {
        Title: 'Ambiente',
        Name: 'Nome',
        Description: 'Descrição',
        IsWeb: 'É Web',
        IsDesktop: 'É Desktop',
        IsProduction: 'É produção',
        IsMobile: 'É Mobile',
        DatabaseName: 'Nome do banco de dados',
        Version: 'Versão',
        Save: 'Salvar',
        Cancel: 'Cancelar',
        Warnings: {
          Warning: 'Atenção!',
          UpdateDbConfirm: 'Estou ciente dos riscos e desejo continuar',
          updateDatabaseName: 'Ao alterar a base de dados, os cadastros da Web e Korp poderão entrar em divergência fazendo com que o sistema perca integridade.',
          updateDatabaseVersion: 'Ao alterar a Versão, o sistema poderá ficar inoperante ou gerar problemas de cadastro.'
        },
        Errors: {
          InvalidDbVersion: 'A versão do banco de dados deve estar no formato correto. Exemplo: 2023.1.2 ou 2023.12.34',
          InvalidDbNameRegexValidate: 'O nome do banco de dados não pode conter caracteres especiais tais como acentos, simbolos e espaços.',
          emptyDatabase: 'O nome do banco é obrigatório quando o campo desktop está marcado',
          unknown: 'Erro desconhecido, tente novamente mais tarde',
          NameConflict: 'Nome com conflito, renomeie para continuar',
          NotFound: 'Não encontrado',
          InProductionConflict: 'Já existe um ambiente destinado para Produção, desmarque para continuar',
          NotTypedEnvironment: 'Marque o ambiente como Desktop ou Web para continuar',
          InvalidDatabaseName: 'Nome do banco inválido, preencha para continuar',
          OrganizationUnitNotFound: 'Erro desconhecido, tente novamente mais tarde',
          IdConflict: 'Conflito de Id'
        }
      },
      Errors: {
        userHasNoAuthorization: 'Usuário sem permissão'
      }
    },
    Environment: {
      EnvironmentUnitsOf: 'Ambientes pertencentes a unidade ',
      EnvironmentUnits: 'Ambientes',
      SelectAnUnitToViewEnvironments: 'Selecione uma unidade para visualizar os ambientes',
      UnitEnvironment: {
        Tooltips: {
          Activate: 'Ativar ambiente',
          Deactivate: 'Desativar ambiente'
        },
        FieldName: {
          Name: 'Nome',
          Description: 'Descrição',
          IsWeb: 'É Web',
          IsDesktop: 'É Desktop',
          IsProduction: 'É produção',
          DatabaseName: 'Nome do banco de dados',
          Version: 'Versão'
        }
      }
    }
  }

}
