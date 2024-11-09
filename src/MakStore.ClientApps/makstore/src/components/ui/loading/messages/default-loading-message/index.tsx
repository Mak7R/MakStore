import React from 'react';
import LoadingMessageWithProgress from "@/components/ui/loading/messages/loading-message-with-progress";


export default function DefaultLoadingMessage(props: {message?: string}) {
  return <LoadingMessageWithProgress {...props}/>
}