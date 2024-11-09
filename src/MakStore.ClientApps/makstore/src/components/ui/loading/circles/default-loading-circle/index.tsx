import React from 'react';
import styles from './index.module.css'
interface DefaultLoadingCircleProps {

}

export default function Index(props: DefaultLoadingCircleProps) {
  return (
    <>
      <div className={styles.container}>
        <div className={styles.ring}></div>
        <div className={styles.ring}></div>
        <div className={styles.ring}></div>
        <div className={styles.ring}></div>
      </div>
    </>
  );
}