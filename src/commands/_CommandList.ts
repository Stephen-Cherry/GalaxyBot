import { Command } from "../interfaces/Command";
import { info } from "./info";
import { server } from "./server";

export const CommandList: Command[] = [info, server];
