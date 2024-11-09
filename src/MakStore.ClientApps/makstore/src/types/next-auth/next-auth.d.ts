import { DefaultSession } from "next-auth"

declare module "next-auth" {
  interface Session {
    user: {
      id?: string | null;
      role?: string | string[] | null
    } & DefaultSession["user"]
    accessToken: string;
  }
}