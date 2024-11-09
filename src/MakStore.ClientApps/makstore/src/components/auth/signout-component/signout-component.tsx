'use client'

import React, {useEffect} from 'react';
import {signOut} from "next-auth/react";
import {useRouter} from "next/navigation";

interface SignOutComponentProps {

}

export default function SignOutComponent(props: SignOutComponentProps) {
  const router = useRouter();
  useEffect(() => {
    signOut()
      .then(_ => {
        
        
        router.push("/");
      })
  }, []);
  
  return (
    <>
      
    </>
  );
}