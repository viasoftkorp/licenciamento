import { LicensingInfoI18N } from "../interfaces/licensing-info-i18n.interface";

export const LICENSING_INFO_I18N_PT: LicensingInfoI18N = {
    LicensingInfo: {
        Active: 'Ativo',
        AssignLicense: 'Atribuir licença',
        TransferLicense: 'Transferir licença',
        RevokeLicense: 'Revogar licença',
        CurrentUser: 'Usuário atual',
        Errors: {
            UnknownError: 'Erro desconhecido',
            NoNamedUser: 'Não foi possível encontrar o usuário',
            NoProduct: 'Não foi possível encontrar o produto licenciado',
            ProductIsNotNamed: 'O modelo da licença não é nomeado',
            NoTenantWithSuchId: 'Não foi possível encontrar o licenciamento',
            TooManyNamedUsers: 'Você não tem mais licenças disponíveis',
            NamedUserEmailAlreadyInUse: 'Não foi possível adicionar a licença nomeada porque o email já está em uso'
        },
        Identifier: 'Identificador',
        LicensingInformation: 'Informações do licenciamento',
        LicensedCnpjs: 'CNPJs licenciados',
        LicensesInUse: 'Licenças em uso',
        UsedLicenses: 'Licenças utilizadas',
        Modes: {
            Online: 'Online',
            Offline: 'Offline'
        },
        Models: {
            Floating: 'Simultânea',
            Named: 'Nomeada'
        },
        Modal: {
            SearchUser: 'Buscar usuário',
            Save: 'Salvar',
            Cancel: 'Cancelar',
            Identifier: 'Identificador'
        },
        Notification: {
            ActionIrreversible: 'Esta ação não poderá ser revertida',
            ConfirmDeletion: 'Você tem certeza de que deseja remover \"{{name}}\"?'
        },
        Product: 'Produto',
        Products: {
            UsedLicenses: 'Licenças utilizadas',
            LicensingModel: 'Tipo de licença',
            LicensingMode: 'Modo',
            Name: 'Nome do produto',
            NumberOfLicenses: 'Quantidade adquirida',
            Products: 'Produtos',
            SubscriptionStatus: 'Status da assinatura',
            Status: {
                ProductBlocked: 'Bloqueado',
                ProductActive: 'Ativo',
                BundleBlocked: 'Bloqueado',
                BundleActive: 'Ativo'
            },
            Users: 'Usuários'
        },
        Status: {
            Active: 'Ativo',
            Blocked: 'Bloqueado',
            NeedsApproval: 'Pendente Aprovação',
            Trial: 'Trial',
            Status: 'Status'
        },
        UsersGrid: {
            TenantId: 'Identificador do Licenciamento',
            User: 'Usuário',
            AppIdentifier: 'Identificador do App',
            AppName: 'Nome do App',
            ProductIdentifier: 'Identificador do produto',
            ProductName: 'Nome do produto',
            SoftwareIdentifier: 'Identificador do Software',
            SoftwareName: 'Nome do Software',
            SoftwareVersion: 'Versão do Software',
            HostName: 'Hostname',
            HostUser: 'Usuário do sistema operacional',
            LocalIPAddress: 'Endereço IP local',
            Language: 'Linguagem',
            OSInfo: 'Informações do sistema',
            BrowserInfo: 'Informações do navegador',
            DatabaseName: 'Nome Base de dados',
            Remove: 'Remover',
            StartTime: 'Horário de acesso',
            AcessDuration: 'Tempo em uso',
            LastUpdate: 'Última atualização',
            CNPJ: 'CNPJ',
            Additionallicense: 'Licença adicional',
            EndTime: 'Horário de saída',
            Status: {
                Status: 'Status',
                Online: 'Online',
                Offline: 'Offline'
            },
            UsageTime: 'Tempo de uso em minutos',
            DeviceIdentifier: 'Identificador do dispositivo',
            LastSeen: 'Visto por último'
        },
        User: 'Usuário'
    }
}