import React from 'react';
import styles from './header-component.module.css';
import {getServerSession} from "next-auth";
import {authOptions} from "@/app/api/auth/[...nextauth]/auth-options";

interface HeaderComponentProps {

}

export default async function HeaderComponent(props: HeaderComponentProps) {
  const session = await getServerSession(authOptions)
  
  return (
    <header className={styles.mainHeader}>
      <ul className={styles.headerLinksList}>
        <li className={styles.headerLinksListItem}>
          <a className={styles.headerLink} href="/">
            Home
          </a>
        </li>
        <li className={styles.headerLinksListItem}>
          <a className={styles.headerLink} href="/products">
            Products
          </a>
        </li>

        {
          session ? 
            <>
              <li className={styles.headerLinksListItem}>
                <a className={styles.headerLink} href="#">
                  {session.user.name}
                </a>
              </li>
              <li className={styles.headerLinksListItem}>
                <a className={styles.headerLink} href="/sign-out">
                  Sign out
                </a>
              </li>
            </> :
            <>
              <li className={styles.headerLinksListItem}>
                <a className={styles.headerLink} href="/sign-in">
                  Sign in
                </a>
              </li>
            </>
        }
      </ul>
    </header>
  );
}