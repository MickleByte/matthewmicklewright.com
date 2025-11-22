import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { format } from 'date-fns';
import { ArrowLeft } from 'lucide-react';
import { Helmet } from 'react-helmet-async';

const ArticlePage = () => {
  const { id } = useParams();
  const [article, setArticle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch('/articles.json')
      .then(res => {
        if (!res.ok) throw new Error('Failed to fetch articles');
        return res.json();
      })
      .then(articles => {
        const foundArticle = articles.find(article => article.id === id);
        if (!foundArticle) {
          throw new Error('Article not found');
        }
        setArticle(foundArticle);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, [id]);

  if (loading) return <div className="loading">Loading article...</div>;
  if (error) return <div className="error">{error}</div>;
  if (!article) {
    return (
      <div className="error">
        <h2>Article Not Found</h2>
        <p>The article you're looking for doesn't exist.</p>
        <Link to="/" className="read-more-btn">Back to Home</Link>
      </div>
    );
  }

  return (
    <>
      <Helmet>
        
      </Helmet>
      <div className="article-page">
        <Link to="/" className="back-link">
          <ArrowLeft size={20} />
          Back to Home
        </Link>
        
        <article>
          <header className="article-header">
            <h1 className="article-page-title">{article.title}</h1>
          </header>

          <img 
            src={article.image} 
            alt={article.title}
            className="article-page-image"
          />

          <div 
            className="article-page-content"
            dangerouslySetInnerHTML={{ __html: article.content }}
          />
        </article>

        
      </div>
    </>
  );
};

export default ArticlePage; 