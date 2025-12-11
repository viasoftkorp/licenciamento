export interface CreateOrUpdateEnvironmentInput { 
    id?: string;
    name?: string | null;
    description?: string | null;
    isProduction?: boolean;
    isMobile?: boolean;
    isWeb?: boolean;
    isDesktop?: boolean;
    databaseName?: string | null;
    organizationUnitId?: string;
    isActive?: boolean;
}
