// Visual Regression Test command
import { addHelperCommands } from '@viasoft/testing';
import { addMatchImageSnapshotCommand } from 'cypress-image-snapshot/command';

addMatchImageSnapshotCommand();
addHelperCommands();