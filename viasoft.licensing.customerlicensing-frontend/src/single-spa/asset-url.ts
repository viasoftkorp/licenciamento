import { isDevMode } from '@angular/core';
import { ensureTrailingSlash } from '@viasoft/http';
import { environment } from 'src/environments/environment';

// In single-spa, the assets need to be loaded from a dynamic (app) location,
export function assetUrl(assetPath: string): string {
  const basePath = String(environment.assetsUrl);
  if (assetPath.startsWith('/')) {
    assetPath = assetPath.substr(1);
  }
  if (assetPath.startsWith('assets/')) {
    if (isDevMode()) {
      console.warn(`AssetUrl - Do not use 'assets/' in your assetPath. Please replace \nfrom  "${assetPath}"\nto    "${assetPath.substr('assets/'.length)}"`);
    }
    assetPath = assetPath.substr('assets/'.length);
  }
  return `${ensureTrailingSlash(basePath)}${assetPath}`;
}
