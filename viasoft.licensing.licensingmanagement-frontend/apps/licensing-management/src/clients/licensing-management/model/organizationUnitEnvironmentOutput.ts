export interface OrganizationUnitEnvironmentOutput { 
    name?: string | null;
    description?: string | null;
    isProduction?: boolean;
    isMobile?: boolean;
    isWeb?: boolean;
    isDesktop?: boolean;
    databaseName?: string | null;
    organizationUnitId?: string;
    organizationId?: string;
    tenantId?: string;
    isActive?: boolean;
    id?: string;
}
