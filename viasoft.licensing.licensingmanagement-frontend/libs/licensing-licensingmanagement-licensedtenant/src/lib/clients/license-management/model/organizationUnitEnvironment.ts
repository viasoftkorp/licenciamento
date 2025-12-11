export interface OrganizationUnitEnvironment { 
    name: string;
    description?: string | null;
    isProduction?: boolean;
    isMobile?: boolean;
    isWeb?: boolean;
    isDesktop?: boolean;
    databaseName?: string | null;
    organizationUnitId?: string;
    isActive?: boolean;
    id?: string;
}
