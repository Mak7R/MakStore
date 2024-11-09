import React from 'react';
import styles from './index.module.css'

interface LoadingMessageWithProgressProps {
  message?: string
}

export default function LoadingMessageWithProgress(props: LoadingMessageWithProgressProps) {
  return (
    <div className={styles.loader}>
      <span className={styles.loaderText}>{props.message ?? "Loading"}</span>
      <span className={styles.load}></span>
    </div>
  );
}