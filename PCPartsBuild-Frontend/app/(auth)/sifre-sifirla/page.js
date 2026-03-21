"use client";
import React, { useState, useEffect, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';
import Link from 'next/link';

function ResetPasswordForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const email = searchParams.get('email');
  let token = searchParams.get('token');

  useEffect(() => {
    if (!email || !token) {
      Swal.fire({
        title: 'Hatalı Link',
        text: 'Lütfen şifremi unuttum işlemini tekrar başlatın.',
        icon: 'error',
        background: '#1f2937', color: '#fff'
      }).then(() => {
        router.push('/sifremi-unuttum');
      });
    }
  }, [email, token, router]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (password !== confirmPassword) {
      Swal.fire({ title: 'Hata', text: 'Şifreler eşleşmiyor!', icon: 'warning', background: '#1f2937', color: '#fff' });
      return;
    }

    setLoading(true);
    try {
      const response = await api.post('/auth/reset-password', {
        email,
        token: token.replace(/ /g, '+'),
        newPassword: password
      });

      if (response.ok) {
        Swal.fire({
          title: 'Başarılı!',
          text: 'Şifreniz güncellendi. Giriş yapabilirsiniz.',
          icon: 'success',
          background: '#1f2937', color: '#fff'
        }).then(() => {
          router.push('/giris');
        });
      } else {
        const result = await response.json();
        Swal.fire({ title: 'Hata', text: result.message || 'Şifre güncellenemedi.', icon: 'error', background: '#1f2937', color: '#fff' });
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen flex items-center justify-center p-6 sm:p-12 overflow-hidden bg-black">
      <video autoPlay loop muted playsInline className="absolute inset-0 w-full h-full object-cover opacity-40 grayscale">
        <source src="/Videos/Video1.webm" type="video/webm" />
      </video>
      <div className="absolute inset-0 bg-gradient-to-t from-black via-transparent to-black/50"></div>

      <div className="relative w-full max-w-md bg-card-darker/90 backdrop-blur-xl p-10 rounded-3xl border border-white/10 shadow-2xl animate-fade-in-down">
        <div className="text-center mb-10">
          <h1 className="text-3xl font-black mb-3 tracking-tight">Yeni Şifre Belirle</h1>
          <p className="text-gray-400 text-sm font-medium">Güçlü bir şifre seçtiğinizden emin olun.</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="space-y-2">
            <label className="text-xs font-black text-gray-500 uppercase tracking-widest ml-1">Yeni Şifre</label>
            <input 
              type="password" 
              required 
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              className="w-full bg-gray-900 border border-white/5 rounded-2xl px-6 py-4 text-white focus:outline-none focus:ring-2 focus:ring-primary transition-all hover:bg-gray-800"
            />
          </div>
          <div className="space-y-2">
            <label className="text-xs font-black text-gray-500 uppercase tracking-widest ml-1">Yeni Şifre (Tekrar)</label>
            <input 
              type="password" 
              required 
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="••••••••"
              className="w-full bg-gray-900 border border-white/5 rounded-2xl px-6 py-4 text-white focus:outline-none focus:ring-2 focus:ring-primary transition-all hover:bg-gray-800"
            />
          </div>
          <button 
            type="submit" 
            disabled={loading}
            className="w-full bg-primary text-white font-black py-4 rounded-2xl shadow-xl shadow-primary/20 hover:brightness-110 active:scale-95 transition-all uppercase tracking-widest text-sm"
          >
            {loading ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
          </button>
        </form>
      </div>
    </div>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={<div className="min-h-screen bg-black flex items-center justify-center"><span className="material-symbols-outlined animate-spin text-4xl text-primary">sync</span></div>}>
      <ResetPasswordForm />
    </Suspense>
  );
}
