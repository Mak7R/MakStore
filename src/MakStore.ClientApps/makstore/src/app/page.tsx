import {getServerSession} from "next-auth";
import {authOptions} from "@/app/api/auth/[...nextauth]/auth-options";
import React from "react";

export default async function HomePage() {
  const session = await getServerSession(authOptions)
  
  return (
    <div className='p-5'>
      <h1>Welcome</h1>

      <h2>Server session</h2>
      {
        session ?
          <>
            <p>Session: {JSON.stringify(session)}</p>
          </> :
          <>
            <p>You are not logged in</p>
          </>
      }
    </div>
  );
}
