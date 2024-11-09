'use client'

import {useSession} from "next-auth/react";
import axios from "axios";


const useAuthAxios = () => {
  const {data: session} = useSession();

  if (session?.accessToken){
    return axios.create({
      headers: {
        "Content-Type": "application/json",
        "Authorization":  `Bearer ${session.accessToken}`
      },
    });
  }

  return axios.create({
    headers: {
      "Content-Type": "application/json"
    },
  });
}

export default useAuthAxios;