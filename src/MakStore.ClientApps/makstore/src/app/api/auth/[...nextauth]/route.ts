import NextAuth from "next-auth";

import DuendeIDS6Provider  from "next-auth/providers/duende-identity-server6";
import parseJWT from "@/utils/jwt-parser";

async function refreshAccessToken(token: any) {
  try {
    const url =
      `${process.env.DUENDE_IDS6_ISSUER}/connect/token`;
      const params = new URLSearchParams({
        client_id: process.env.DUENDE_IDS6_ID!,
        grant_type: "refresh_token",
        refresh_token: token.refreshToken,
      })

    const response = await fetch(url, {
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      method: "POST",
      body: params
    })

    console.log("Refreshing access token...", url);

    const refreshedTokens = await response.json()

    if (!response.ok) {
      throw refreshedTokens
    }
    
    console.log("Refreshing access token successfully finished...");
    
    return {
      ...token,
      accessToken: refreshedTokens.access_token,
      accessTokenExpires: Date.now() + refreshedTokens.expires_in * 1000,
      refreshToken: refreshedTokens.refresh_token ?? token.refreshToken,
    }
  } catch (error) {
    console.error(error)

    return {
      ...token,
      error: "RefreshAccessTokenError",
    }
  }
}


export const authOptions = {
  providers: [
    DuendeIDS6Provider({
      clientId: process.env.DUENDE_IDS6_ID!,
      clientSecret: "",
      issuer: process.env.DUENDE_IDS6_ISSUER,
      authorization: {
        params: {
          scope: "openid profile email offline_access products_api orders_api role",
        },
      },
    }),
  ],
  callbacks: {
    async signIn({ user, account }: any) {
      if (account?.access_token) {
        const decodedToken = parseJWT(account.access_token);
        user.id = decodedToken.sub;
        user.name = decodedToken.name;
        user.email = decodedToken.email;
        user.role = decodedToken.role;
      }
      
      return true
    },
    async jwt({ token, account }: any) {
      if (account) {
        token.accessToken = account.access_token
        token.refreshToken = account.refresh_token
        // multiply by 1000 to convert the date to milliseconds and minus 5 seconds to not return an expired token
        token.accessTokenExpires = parseJWT(account.access_token).exp * 1000 - 5000
      }

      if (Date.now() < token.accessTokenExpires) {
        return token;
      }
      
      return await refreshAccessToken(token);
    },
    async session({ session, token, user }: any){
      if (!user){
        const decodedToken = parseJWT(token.accessToken);
        user = {id: decodedToken.sub, name: decodedToken.name, email: decodedToken.email, role: decodedToken.role};
      }
      
      session.accessToken = token.accessToken;
      session.user = user;
      
      return session
    },
    async redirect({ url, baseUrl }: any) {
      return baseUrl
    },
  },
}

const handler = NextAuth(authOptions);

export {handler as GET, handler as POST}