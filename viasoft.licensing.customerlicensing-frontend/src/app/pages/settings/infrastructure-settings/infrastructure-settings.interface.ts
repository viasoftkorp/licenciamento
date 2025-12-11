export class InfrastructureSettings {
  gatewayAddress?: string;
  deployVersions?: DeployVersions[];
  licensedTenantId?: string;
}

export class DeployCommand {
  deployCommand: string;
}

export class UpdateVersionCommand {
  updateVersionCommand: string;
}

export class UninstallVersionCommand {
  uninstallVersionCommand: string;
}

export class DeployVersions {
  version: string;
}

