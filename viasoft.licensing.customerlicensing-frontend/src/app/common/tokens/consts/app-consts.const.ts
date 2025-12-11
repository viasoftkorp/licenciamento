import { environment } from 'src/environments/environment';

export class AppConsts {
    public static apiGateway(): string {
        return environment['settings']['backendUrl'];
    }
}
