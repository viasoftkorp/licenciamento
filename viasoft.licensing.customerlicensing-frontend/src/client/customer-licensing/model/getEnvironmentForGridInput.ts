export interface getEnvironmentForGridInput {
  identifier: string,
  unitId: string,
  activeOnly?: boolean,
  desktopOnly?: boolean,
  webOnly?: boolean,
  productionOnly?: boolean,
  sorting?: string
}
