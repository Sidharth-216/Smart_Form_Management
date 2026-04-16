import { createContext, useContext, useEffect, useState } from 'react'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [ready, setReady] = useState(false)

  useEffect(() => {
    try {
      const raw = localStorage.getItem('form-platform-user')
      if (raw) {
        setUser(JSON.parse(raw))
      }
    } catch {
      localStorage.removeItem('form-platform-user')
      localStorage.removeItem('form-platform-token')
    }
    setReady(true)
  }, [])

  const signIn = (authResponse) => {
    localStorage.setItem('form-platform-token', authResponse.token)
    localStorage.setItem('form-platform-user', JSON.stringify(authResponse))
    setUser(authResponse)
  }

  const signOut = () => {
    localStorage.removeItem('form-platform-token')
    localStorage.removeItem('form-platform-user')
    setUser(null)
  }

  return <AuthContext.Provider value={{ user, ready, signIn, signOut }}>{children}</AuthContext.Provider>
}

export function useAuth() {
  return useContext(AuthContext)
}
